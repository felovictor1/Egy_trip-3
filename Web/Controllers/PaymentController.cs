using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using eg_travil.models;
using Microsoft.EntityFrameworkCore;

namespace eg_travil.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public PaymentController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /// <summary>
        /// Creates a Stripe Checkout session for the hotel cost of a saved plan.
        /// Charge = HotelPricePerNight × Nights in EGP (piastres × 100 for Stripe).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateCheckoutSession(int savedPlanId)
        {
            // 1. Set Stripe secret key
            Stripe.StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            // 2. Fetch the saved plan
            var plan = await _context.SavedPlans.FindAsync(savedPlanId);
            if (plan == null) return NotFound();

            // 3. Calculate total hotel cost (price/night × nights)
            decimal totalEgp = plan.HotelPricePerNight * plan.Nights;
            if (totalEgp <= 0) totalEgp = 1; // Safety: Stripe requires amount > 0

            // Stripe expects amount in the smallest currency unit.
            // For EGP: 1 EGP = 100 piastres → multiply by 100
            long amountPiastres = (long)(totalEgp * 100);

            // 4. Build the domain for redirect URLs
            var request = HttpContext.Request;
            var domain = $"{request.Scheme}://{request.Host}";

            // 5. Configure Stripe session
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = amountPiastres,
                            Currency = "egp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = plan.HotelName ?? "Hotel Booking",
                                Description = $"{plan.Nights} night(s) × {plan.HotelPricePerNight:N2} EGP/night"
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + $"/Payment/Success?savedPlanId={savedPlanId}",
                CancelUrl  = domain + $"/Payment/Cancel?savedPlanId={savedPlanId}",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            // 6. Redirect user to Stripe hosted checkout
            Response.Headers.Append("Location", session.Url);
            return new StatusCodeResult(303);
        }

        /// <summary>
        /// Stripe redirects here on successful payment. Marks the plan as paid.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Success(int savedPlanId)
        {
            var plan = await _context.SavedPlans.FindAsync(savedPlanId);
            if (plan != null)
            {
                plan.IsPaid = true;
                _context.SavedPlans.Update(plan);
                await _context.SaveChangesAsync();
            }

            ViewData["SavedPlanId"]  = savedPlanId;
            ViewData["HotelName"]    = plan?.HotelName  ?? "Hotel";
            ViewData["TotalAmount"]  = plan != null ? (plan.HotelPricePerNight * plan.Nights).ToString("N2") : "0";
            ViewData["Nights"]       = plan?.Nights ?? 1;
            ViewData["CurrentTitle"] = plan?.Title ?? "";
            return View();
        }

        /// <summary>
        /// Updates the SavedPlan title so the user can recognise it on My Plans.
        /// Called via AJAX from the Success page.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RenamePlan([FromBody] RenamePlanRequest req)
        {
            if (req == null || req.SavedPlanId <= 0 || string.IsNullOrWhiteSpace(req.Title))
                return BadRequest(new { success = false, message = "Invalid request." });

            var plan = await _context.SavedPlans.FindAsync(req.SavedPlanId);
            if (plan == null) return NotFound(new { success = false });

            plan.Title = req.Title.Trim();
            _context.SavedPlans.Update(plan);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        public record RenamePlanRequest(int SavedPlanId, string Title);

        /// <summary>
        /// Stripe redirects here if the user cancels the checkout.
        /// </summary>
        [HttpGet]
        public IActionResult Cancel(int savedPlanId)
        {
            ViewData["SavedPlanId"] = savedPlanId;
            return View();
        }
    }
}
