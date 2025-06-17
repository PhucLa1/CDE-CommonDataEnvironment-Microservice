using AI_LLM.Service;
using Microsoft.AspNetCore.Mvc;

namespace AI_LLM.Controllers
{
    [ApiController]
    [Route("gemini")]
    public class GeminiController(IGeminiService geminiService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AskQuestion([FromBody] GeminiAskRequest geminiAskRequest)
        {
            return Ok(await geminiService.AskGeminiAsync(geminiAskRequest));
        }

        [HttpGet]
        [Route("conversations/{projectId}")]
        public async Task<IActionResult> GetConversationByProjectId(int projectId)
        {
            var res = await geminiService.GetConversationByProjectId(projectId);
            return Ok(res);
        }
    }
}
