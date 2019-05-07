using System.Collections.Generic;

namespace Framework.Interfaces.Tasks
{
    public interface IServiceBatchTask<TBatchItemType> : IServiceTask
    {
        void OnExecuteTaskComplete(List<TBatchItemType> processedItems);
    }
}