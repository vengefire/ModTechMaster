using System;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using Dapper;
using Framework.Domain.Queue;
using Framework.Interfaces.Data.Services;
using Framework.Interfaces.Repositories;
using Framework.Logic.Repository;
using MessageAudit = Framework.Data.MessageQueue.Model.MessageAudit;
using MessageProcessingError = Framework.Data.MessageQueue.Model.MessageProcessingError;

namespace Framework.Data.MessageQueue
{
    using DalMessageAudit = MessageAudit;
    using DalMessageProcessingError = MessageProcessingError;
    using DomainMessageAudit = Domain.Queue.MessageAudit;
    using DomainMessageProcessingError = Domain.Queue.MessageProcessingError;

    public class MessageQueueService : IMessageQueueService
    {
        private readonly IDapperRepository<DalMessageAudit> _messageAuditRepository;

        private readonly IDapperRepository<DalMessageProcessingError> _messageProcessingErrorRepository;

        static MessageQueueService()
        {
            InitTypeMaps();
        }

        public MessageQueueService(
            IDapperRepository<DalMessageAudit> messageAuditRepository,
            IDapperRepository<DalMessageProcessingError> messageProcessingErrorRepository)
        {
            _messageAuditRepository = messageAuditRepository;
            _messageProcessingErrorRepository = messageProcessingErrorRepository;
        }

        public MessageQueueService(string connectionString)
        {
            _messageAuditRepository = new DapperRepositoryBase<DalMessageAudit>(new SqlConnection(connectionString));
            _messageProcessingErrorRepository =
                new DapperRepositoryBase<DalMessageProcessingError>(new SqlConnection(connectionString));
        }

        public long CreateMessageAudit(DomainMessageAudit messageAudit)
        {
            var dalMessageAudit = Mapper.Map<DalMessageAudit>(messageAudit);
            _messageAuditRepository.Create(dalMessageAudit);
            return dalMessageAudit.Id;
        }

        public void UpdateMessageProcessedStats(string messageId, MessageStatus messageStatus, long processingTime)
        {
            var dalMessageAudit = GetMessageAuditByMessageId(messageId);

            if (null == dalMessageAudit) return;

            dalMessageAudit.MessageStatusId = (int) messageStatus;
            dalMessageAudit.ProcessingTime = processingTime;
            _messageAuditRepository.Update(dalMessageAudit);
        }

        public void CreateMessageAuditException(DomainMessageProcessingError processingError)
        {
            var dalProcessingError = Mapper.Map<MessageProcessingError>(processingError);
            _messageProcessingErrorRepository.Create(dalProcessingError);
        }

        public DomainMessageAudit GetMessageAuditById(long id)
        {
            return Mapper.Map<DomainMessageAudit>(_messageAuditRepository.FetchByKey(id));
        }

        private static void InitTypeMaps()
        {
            Mapper.CreateMap<DomainMessageAudit, DalMessageAudit>()
                .ForMember(dest => dest.MessageStatusId, opt => opt.MapFrom(src => (int) src.MessageStatus))
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

            return _messageAuditRepository.DbConnection.Query<DalMessageAudit>(
                sql,
                new
                {
                    MessageId = messageId
                }).FirstOrDefault();
        }
    }
}