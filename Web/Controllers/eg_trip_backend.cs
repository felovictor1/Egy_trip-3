using Microsoft.AspNetCore.Mvc;

namespace eg_trip_backend.Controllers // تأكد من اسم الـ namespace عندك
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTest()
        {
            // هنبعت رسالة بسيطة عشان نتأكد من الربط
            return Ok(new { message = "الربط بنجاح يا بطل! الباك-أند شغال" });
        }
    }
}