namespace Framework.Interfaces.Data.Services
{
    using Domain.Services;

    public interface IServiceMethodLogService
    {
        int AddLog(ref ServiceMethodLog logEntry);

        void UpdateLog(int id, string responseMessage, long processingTime);
    }
}