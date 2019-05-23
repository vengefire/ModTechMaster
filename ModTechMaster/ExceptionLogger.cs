namespace ModTechMaster.UI
{
    using System;

    using Castle.Core.Logging;

    using Framework.Interfaces.Logging;

    public class ExceptionLogger : IExceptionLogger
    {
        private readonly ILogger logger;

        public ExceptionLogger(ILogger logger)
        {
            this.logger = logger;
        }

        public void Log(Exception ex)
        {
            this.logger.Error(ex.ToString(), ex);
        }
    }
}