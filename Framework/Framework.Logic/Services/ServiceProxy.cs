using System.ServiceProcess;
using Framework.Interfaces.Services;

namespace Framework.Logic.Services
{
    public class ServiceProxy : ServiceBase
    {
        private readonly IService engineService;

        public ServiceProxy(IService engineService)
        {
            this.engineService = engineService;
            ServiceName = this.engineService.ServiceName();
        }

        protected override void OnStart(string[] args)
        {
            engineService.OnStart();
        }

        protected override void OnStop()
        {
            engineService.OnStop();
        }
    }
}