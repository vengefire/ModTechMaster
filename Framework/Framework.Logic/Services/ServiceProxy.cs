namespace Framework.Logic.Services
{
    using System.ServiceProcess;
    using Interfaces.Services;

    public class ServiceProxy : ServiceBase
    {
        private readonly IService engineService;

        public ServiceProxy(IService engineService)
        {
            this.engineService = engineService;
            this.ServiceName = this.engineService.ServiceName();
        }

        protected override void OnStart(string[] args)
        {
            this.engineService.OnStart();
        }

        protected override void OnStop()
        {
            this.engineService.OnStop();
        }
    }
}