using Framework.Domain.Services;
using Framework.Interfaces.Data.Services;

namespace Framework.Logic.Services
{
    public class NullServiceMethodLogService : IServiceMethodLogService
    {
        private static readonly IServiceMethodLogService Singleton = new NullServiceMethodLogService();

        public int AddLog(ref ServiceMethodLog logEntry)
        {
            return 0;
        }

        public void UpdateLog(int id, string responseMessage, long processingTime)
        {
        }

        public static IServiceMethodLogService Instance()
        {
            return Singleton;
        }
    }
}