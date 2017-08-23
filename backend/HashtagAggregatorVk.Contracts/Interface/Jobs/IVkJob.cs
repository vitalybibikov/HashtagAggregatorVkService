using System.Threading.Tasks;
using HashtagAggregator.Core.Contracts.Interface.Cqrs.Command;
using HashtagAggregator.Service.Contracts.Jobs;

namespace HashtagAggregatorVk.Contracts.Interface.Jobs
{
    public interface IVkJob: IJob
    {
        Task<ICommandResult> Execute(VkJobTask task);
    }
}
