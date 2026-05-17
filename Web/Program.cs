using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using eg_travil.models; // استدعاء الموديلز
using eg_travil.servecies;
using eg_travil.services; // BudgetCalculatorService

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة الـ DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. إعداد الـ Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("jwt"))
                {
                    context.Token = context.Request.Cookies["jwt"];
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers();
builder.Services.AddRazorPages();

// تسجيل الخدمات الخاصة بك
builder.Services.AddHttpClient(); // Register global IHttpClientFactory for TripMlController
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddHttpClient<eg_travil.services.WeatherService>(); // Typed client for WeatherAPI
builder.Services.AddSingleton<BudgetCalculatorService>(); // Stateless — singleton is ideal

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed Admin User
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Automatically apply migrations and create database if it doesn't exist
        context.Database.Migrate();

        if (!context.Users.Any(u => u.Email == "felopater@gmail.com"))
        {
            context.Users.Add(new User 
            { 
                FullName = "Admin", 
                Email = "felopater@gmail.com", 
                Password = BCrypt.Net.BCrypt.HashPassword("Egytrip1+") 
            });
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding database: {ex.Message}");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// تفعيل الـ CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

        app.MapDefaultControllerRoute(); // enables conventional: {controller}/{action}/{id?}
        app.MapControllers();            // keeps attribute-routed API controllers
app.MapRazorPages();

app.Run();