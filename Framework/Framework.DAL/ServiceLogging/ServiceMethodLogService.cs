using AutoMapper;
using Framework.Domain.Services;
using Framework.Interfaces.Data.Services;
using Framework.Interfaces.Repositories;
using DalServiceLog = Framework.Data.ServiceLogging.Model.ServiceMethodLog;

namespace Framework.Data.ServiceLogging
{
    public class ServiceMethodLogService : IServiceMethodLogService
    {
        private readonly IMapper _mapper;
        private readonly IDapperRepository<DalServiceLog> _serviceMethodLogRepository;

        public ServiceMethodLogService(IDapperRepository<DalServiceLog> serviceMethodLogRepository)
        {
            _serviceMethodLogRepository = serviceMethodLogRepository;
            var mapConfig =
                new MapperConfiguration(configuration => { configuration.CreateMap<ServiceMethodLog, DalServiceLog>(); });
            _mapper = mapConfig.CreateMapper();
        }

        public int AddLog(ref ServiceMethodLog logEntry)
        {
            var dalLogEntry = _mapper.Map<DalServiceLog>(logEntry);
            _serviceMethodLogRepository.Create(dalLogEntry);
            logEntry.Id = dalLogEntry.Id;
            return logEntry.Id;
        }

        public void UpdateLog(int id, string responseMessage, long processingTime)
        {
            var logEntry = _serviceMethodLogRepository.FetchByKey(id);
            logEntry.ResponseMessage = responseMessage;
            _serviceMethodLogRepository.Update(logEntry);
        }
    }
}