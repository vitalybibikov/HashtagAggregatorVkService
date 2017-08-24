using System.Threading.Tasks;
using HashtagAggregator.Core.Contracts.Interface.Cqrs.Command;
using HashtagAggregator.Core.Entities.VkEntities;

namespace HashtagAggregatorVk.Contracts.Interface.Queues
{
    public interface IVkQueue
    {
        Task<ICommandResult> Enqueue(VkNewsFeed message);
    }
}