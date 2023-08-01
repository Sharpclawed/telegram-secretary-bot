using System.Threading;
using System.Threading.Tasks;

namespace TrunkRings.Reporters
{
    interface IReporter
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}