using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretaryWebAPI.Models;
using TelegramBotTry1;

namespace SecretaryWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistributeController : ControllerBase
    {
        private readonly ISecretaryBot bot;

        public DistributeController(ISecretaryBot bot)
        {
            this.bot = bot;
        }

        //todo auth
        [HttpPost]
        [Route("messages")]
        public async Task<IActionResult> Update([FromBody] DistributeMessages distributeMessages)
        {
            //todo store commands
            //todo check sending
            //todo return result
            await bot.BotCommander.SendMessagesAsync(distributeMessages.ChatId, distributeMessages.Text);

            return Ok();
        }
    }
}