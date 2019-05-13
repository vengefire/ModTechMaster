using System;
using Castle.Core.Logging;
using Framework.Interfaces.Logging;

namespace MTM
{
    public class ExceptionLogger : IExceptionLogger
    {
        private readonly ILogger _logger;

        public ExceptionLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(Exception ex)
        {
            _logger.Error(ex.ToString(), ex);
        }
    }
}