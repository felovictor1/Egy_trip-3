using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using eg_travil.models; // استدعاء الموديلز للتأكد من رؤية User و ApplicationDbContext
using BCrypt.Net;

namespace eg_travil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // --- تسجيل الدخول عبر جوجل ---
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
            logger.LogInformation("Received Google login request.");

            if (string.IsNullOrEmpty(request.IdToken))
            {
                logger.LogWarning("Google login failed: IdToken is null or empty.");
                return BadRequest(new { message = "IdToken is required." });
            }

            try
            {
                // التحقق من التوكن القادم من جوجل
                logger.LogInformation("Validating Google ID Token...");
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _config["Authentication:Google:ClientId"] }
                });

                if (payload == null)
                {
                    logger.LogWarning("Google token validation returned null payload.");
                    return Unauthorized(new { message = "Google token validation failed." });
                }

                logger.LogInformation("Google token validated successfully for email: {Email}", payload.Email);

                // البحث عن المستخدم بالإيميل
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

                if (user == null)
                {
                    logger.LogInformation("User with email {Email} not found. Creating a new account.", payload.Email);
                    // إنشاء حساب جديد تلقائياً في حالة تسجيل الدخول لأول مرة بجوجل
                    user = new User
                    {
                        Email = payload.Email,
                        FullName = payload.Name ?? "Google User",
                        // إنشاء كلمة مرور عشوائية مشفرة لأن الحساب يعتمد على جوجل
                        Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString())
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    logger.LogInformation("New user {Email} created successfully with ID: {Id}", user.Email, user.Id);
                }
                else
                {
                    logger.LogInformation("Existing user found for email: {Email}. Proceeding with login/linking.", payload.Email);
                    // Note: In custom implementations without an external logins table, 
                    // finding the user by email is usually sufficient to "link" them 
                    // as they can now log in via Google.
                }

                var token = GenerateJwtToken(user);
                logger.LogInformation("JWT token generated for user: {Email}", user.Email);
                
                Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true, Expires = DateTime.Now.AddDays(7) });
                
                bool isProfileComplete = user.Age.HasValue && !string.IsNullOrEmpty(user.PhoneNumber);
                return Ok(new { token, fullName = user.FullName, isProfileComplete, message = "Google login successful!" });
            }
            catch (InvalidJwtException ex)
            {
                logger.LogError(ex, "Invalid Google token signature or expired token.");
                return Unauthorized(new { message = "Invalid or expired Google token", detail = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred during Google login.");
                return StatusCode(500, new { message = "An error occurred while processing the Google login", detail = ex.Message });
            }
        }

        // --- إنشاء حساب جديد ---
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] User user)
        {
            // التأكد من عدم تكرار الإيميل
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest(new { message = "Email is already registered!" });

            // التحقق من قوة كلمة المرور
            if (!IsValidPassword(user.Password))
                return BadRequest(new { message = "Password too weak! Must be at least 8 characters with upper, lower, and numbers." });

            // تشفير كلمة المرور قبل الحفظ
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully!" });
        }

        // --- تسجيل الدخول العادي ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            // التحقق من صحة الإيميل وكلمة المرور المشفرة
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized(new { message = "Invalid email or password!" });

            var token = GenerateJwtToken(user);
            Response.Cookies.Append("jwt", token, new CookieOptions { HttpOnly = true, Expires = DateTime.Now.AddDays(7) });
            bool isProfileComplete = user.Age.HasValue && !string.IsNullOrEmpty(user.PhoneNumber);
            return Ok(new { token, fullName = user.FullName, isProfileComplete });
        }

        // --- توليد JWT Token ---
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullName", user.FullName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // صلاحية التوكن 7 أيام
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // --- التحقق من قوة كلمة المرور بالـ Regex ---
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8) return false;
            // لازم تحتوي على حرف كبير، حرف صغير، ورقم
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
        }
    }

    // الكلاسات المساعدة لاستقبال البيانات (DTOs)
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}