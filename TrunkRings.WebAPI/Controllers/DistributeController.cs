using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrunkRings.WebAPI.Models;
using TrunkRings.WebAPI.Services;

namespace TrunkRings.WebAPI.Controllers
{
    public class DistributeController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Messages([FromServices] HandleDistributeMessagesService handleService, [FromBody] DistributeMessages distributeMessages)
        {
            var callerIp = Request.Headers["X-Forwarded-For"];
            var distributeMessageResults = await handleService.EchoAsync(distributeMessages, callerIp);

            var successfulResultsCount = distributeMessageResults.Count(res => res.Verdict == "Success");
            if (successfulResultsCount == 0)
                return new StatusCodeResult(500);

            if (successfulResultsCount == distributeMessageResults.Count)
                return Ok();

            var responseBody = JsonSerializer.Serialize(distributeMessageResults.Select(res => new ResponseResult( res.ChatId, res.Verdict)));
            return new ContentResult {Content = responseBody, ContentType = "application/json", StatusCode = 207};
        }

        private class ResponseResult
        {
            public ResponseResult(long chatId, string verdict)
            {
                ChatId = chatId;
                Status = verdict == "Success" ? 200 : 500;
            }

            public long ChatId { get; }
            public int Status { get; }
        }
    }
}