using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TrunkRings.WebAPI.Services;

namespace TrunkRings.WebAPI.Controllers
{
    public class WebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService, [FromBody] Update update)
        {
            if (update != null)
                await handleUpdateService.EchoAsync(update);

            return Ok();
        }
    }
}
