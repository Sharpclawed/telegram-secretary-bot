using System.Threading.Tasks;

namespace TelegramBotTry1.Commands
{
    public interface IBotCommand
    {
        Task ProcessAsync();
    }
}