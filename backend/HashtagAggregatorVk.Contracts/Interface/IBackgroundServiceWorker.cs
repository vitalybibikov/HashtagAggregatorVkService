using System.Threading.Tasks;
using HashtagAggregator.Core.Contracts.Interface.Cqrs.Command;

namespace HashtagAggregatorVk.Contracts.Interface
{
    public interface IBackgroundServiceWorker
    {
        Task<ICommandResult> Start(string tag);

        ICommandResult Stop(string tag);
    }
}