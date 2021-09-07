using System.Threading.Tasks;

namespace TrunkRings.Commands
{
    public interface IBotCommand
    {
        Task ProcessAsync();
    }
}