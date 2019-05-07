using Framework.Domain.Services;

namespace Framework.Interfaces.Data.Services
{
    public interface IServiceMethodLogService
    {
        int AddLog(ref ServiceMethodLog logEntry);
        void UpdateLog(int id, string responseMessage, long processingTime);
    }
}