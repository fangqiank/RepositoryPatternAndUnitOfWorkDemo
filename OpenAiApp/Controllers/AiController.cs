using Microsoft.AspNetCore.Mvc;
using OpenAiApp.Services;

namespace OpenAiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly ILogger<AiController> _logger;
        private readonly IOpenAiService _service;

        public AiController(ILogger<AiController> logger, IOpenAiService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        [Route("CompleteSentence")]
        public async Task<IActionResult> CompleteSentence(string text)
        {
            var result = await _service.CompleteSentence(text);

            return Ok(result);
        }

        [HttpPost]
        [Route("Adnanced")]
        public async Task<IActionResult> CompleteSentenceAdvanced(string text)
        {
            var result = await _service.CompleteSentenceAdvance(text);

            return Ok(result);
        }

        [HttpPost]
        [Route("AskQuestion")]
        public async Task<IActionResult> AskQuestion(string language)
        {
            var result = await _service.CheckProgrammingLanguage(language);

            return Ok(result);
        }
    }
}
