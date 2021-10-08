using System.Threading.Tasks;

namespace TrunkRings.Commands
{
    interface IBotCommand
    {
        Task ProcessAsync();
    }
}