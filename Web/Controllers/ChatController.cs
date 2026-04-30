using Microsoft.AspNetCore.Mvc;
using eg_travil.models;
using eg_travil.servecies;

namespace eg_travil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public ChatController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (request == null || request.Messages == null || request.Messages.Count == 0)
            {
                return BadRequest("Invalid chat request.");
            }

            try
            {
                var responseMessage = await _geminiService.ChatWithGeminiAsync(request);
                return Ok(new { response = responseMessage });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
