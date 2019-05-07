namespace Framework.Interfaces.Tasks
{
    using System.Collections.Generic;

    public interface IServiceBatchTask<TBatchItemType> : IServiceTask
    {
        void OnExecuteTaskComplete(List<TBatchItemType> processedItems);
    }
}