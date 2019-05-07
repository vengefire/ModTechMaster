namespace Framework.Logic.Services
{
    using Domain.Services;
    using Interfaces.Data.Services;

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
            return NullServiceMethodLogService.Singleton;
        }
    }
}