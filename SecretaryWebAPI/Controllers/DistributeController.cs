using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretaryWebAPI.Models;
using SecretaryWebAPI.Services;

namespace SecretaryWebAPI.Controllers
{
    public class DistributeController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Messages([FromServices] HandleDistributeMessagesService handleService, [FromBody] DistributeMessages distributeMessages)
        {
            var callerIp = Request.Headers["X-Forwarded-For"];
            await handleService.EchoAsync(distributeMessages, callerIp);

            return Ok();
        }
    }
}