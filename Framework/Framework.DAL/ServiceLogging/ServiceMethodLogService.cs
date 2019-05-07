using DalServiceLog = Framework.Data.ServiceLogging.Model.ServiceMethodLog;

namespace Framework.Data.ServiceLogging
{
    using AutoMapper;
    using Domain.Services;
    using Interfaces.Data.Services;
    using Interfaces.Repositories;

    public class ServiceMethodLogService : IServiceMethodLogService
    {
        private readonly IMapper _mapper;
        private readonly IDapperRepository<DalServiceLog> _serviceMethodLogRepository;

        public ServiceMethodLogService(IDapperRepository<DalServiceLog> serviceMethodLogRepository)
        {
            this._serviceMethodLogRepository = serviceMethodLogRepository;
            var mapConfig =
                new MapperConfiguration(configuration => { configuration.CreateMap<ServiceMethodLog, DalServiceLog>(); });
            this._mapper = mapConfig.CreateMapper();
        }

        public int AddLog(ref ServiceMethodLog logEntry)
        {
            var dalLogEntry = this._mapper.Map<DalServiceLog>(logEntry);
            this._serviceMethodLogRepository.Create(dalLogEntry);
            logEntry.Id = dalLogEntry.Id;
            return logEntry.Id;
        }

        public void UpdateLog(int id, string responseMessage, long processingTime)
        {
            var logEntry = this._serviceMethodLogRepository.FetchByKey(id);
            logEntry.ResponseMessage = responseMessage;
            this._serviceMethodLogRepository.Update(logEntry);
        }
    }
}