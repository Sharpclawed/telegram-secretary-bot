using System.Threading.Tasks;

namespace TrunkRings.Commands
{
    interface IBotCommand
    {
        Task ProcessAsync();
    }

    interface IBotCommand<T>
    {
        Task<T> ProcessAsync();
    }
}