namespace Framework.Data.MessageQueue
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using AutoMapper;
    using Dapper;
    using Domain.Queue;
    using Interfaces.Data.Services;
    using Interfaces.Repositories;
    using Logic.Repository;
    using DalMessageAudit = Model.MessageAudit;
    using DalMessageProcessingError = Model.MessageProcessingError;
    using DomainMessageAudit = Domain.Queue.MessageAudit;
    using DomainMessageProcessingError = Domain.Queue.MessageProcessingError;

    public class MessageQueueService : IMessageQueueService
    {
        private readonly IDapperRepository<DalMessageAudit> _messageAuditRepository;

        private readonly IDapperRepository<DalMessageProcessingError> _messageProcessingErrorRepository;

        static MessageQueueService()
        {
            MessageQueueService.InitTypeMaps();
        }

        public MessageQueueService(
            IDapperRepository<DalMessageAudit> messageAuditRepository,
            IDapperRepository<DalMessageProcessingError> messageProcessingErrorRepository)
        {
            this._messageAuditRepository = messageAuditRepository;
            this._messageProcessingErrorRepository = messageProcessingErrorRepository;
        }

        public MessageQueueService(string connectionString)
        {
            this._messageAuditRepository = new DapperRepositoryBase<DalMessageAudit>(new SqlConnection(connectionString));
            this._messageProcessingErrorRepository =
                new DapperRepositoryBase<DalMessageProcessingError>(new SqlConnection(connectionString));
        }

        public long CreateMessageAudit(DomainMessageAudit messageAudit)
        {
            var dalMessageAudit = Mapper.Map<DalMessageAudit>(messageAudit);
            this._messageAuditRepository.Create(dalMessageAudit);
            return dalMessageAudit.Id;
        }

        public void UpdateMessageProcessedStats(string messageId, MessageStatus messageStatus, long processingTime)
        {
            var dalMessageAudit = this.GetMessageAuditByMessageId(messageId);

            if (null == dalMessageAudit)
            {
                return;
            }

            dalMessageAudit.MessageStatusId = (int)messageStatus;
            dalMessageAudit.ProcessingTime = processingTime;
            this._messageAuditRepository.Update(dalMessageAudit);
        }

        public void CreateMessageAuditException(DomainMessageProcessingError processingError)
        {
            var dalProcessingError = Mapper.Map<DalMessageProcessingError>(processingError);
            this._messageProcessingErrorRepository.Create(dalProcessingError);
        }

        public DomainMessageAudit GetMessageAuditById(long id)
        {
            return Mapper.Map<DomainMessageAudit>(this._messageAuditRepository.FetchByKey(id));
        }

        private static void InitTypeMaps()
        {
            Mapper.CreateMap<DomainMessageAudit, DalMessageAudit>()
                  .ForMember(dest => dest.MessageStatusId, opt => opt.MapFrom(src => (int)src.MessageStatus))
                  .ForMember(dest => dest.TmStamp, opt => opt.MapFrom(src => DateTime.Now));
            Mapper.CreateMap<DalMessageAudit, DomainMessageAudit>()
                  .ForMember(dest => dest.MessageStatus, opt => opt.MapFrom(src => src.MessageStatusId));
            Mapper.CreateMap<DomainMessageProcessingError, DalMessageProcessingError>();
        }

        private DalMessageAudit GetMessageAuditByMessageId(string messageId)
        {
            var sql =
                "SELECT * " +
                "FROM DataExchange.MessageAudit WITH(READUNCOMMITTED) " +
                "WHERE MessageId = @MessageId";

            return this._messageAuditRepository.DbConnection.Query<DalMessageAudit>(
                                                                                    sql,
                                                                                    new {MessageId = messageId}).FirstOrDefault();
        }
    }
}