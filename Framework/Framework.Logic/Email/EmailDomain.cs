namespace Framework.Logic.Email
{
    using System;
    using AutoMapper;
    using Domain.Email.Models;
    using Interfaces.Email;
    using WebServiceProxy;

    public class EmailDomain : IEmailDomain
    {
        private readonly IEmailPersistence databasePersister;

        private readonly IEmailPersistence diskPersister;

        private readonly EmailServiceProxy emailServiceProxy;

        static EmailDomain()
        {
            Mapper.CreateMap<EmailFileAttachment, Attachment>()
                  .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Name))
                  .ForMember(dest => dest.Bytes, opt => opt.MapFrom(src => src.Content));
            Mapper.CreateMap<Email, EmailMessage>()
                  .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.FileAttachments))
                  .ForMember(dest => dest.BCCAddress, opt => opt.MapFrom(src => src.BCCAddresses))
                  .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
                  .ForMember(
                             dest => dest.BodyFormat,
                             opt => opt.ResolveUsing(src => src.IsContentHtml ? Format.HTML : Format.Text))
                  .ForMember(dest => dest.BodyFormatSpecified, opt => opt.UseValue(true))
                  .ForMember(dest => dest.CCAddress, opt => opt.MapFrom(src => src.CCAddresses))
                  .ForMember(dest => dest.FromAddress, opt => opt.MapFrom(src => src.FromAddress))
                  .ForMember(dest => dest.MailPriority, opt => opt.UseValue(Priority.Medium))
                  .ForMember(dest => dest.MailPrioritySpecified, opt => opt.UseValue(true))
                  .ForMember(dest => dest.NotificationOptions, opt => opt.UseValue(DeliveryOptions.None))
                  .ForMember(dest => dest.NotificationOptionsSpecified, opt => opt.UseValue(true))
                  .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject))
                  .ForMember(dest => dest.ToAddress, opt => opt.MapFrom(src => src.ToAddresses));
        }

        public EmailDomain(IEmailPersistence diskPersister, IEmailPersistence databasePersister)
        {
            this.emailServiceProxy = new EmailServiceProxy();
            this.diskPersister = diskPersister;
            this.databasePersister = databasePersister;
        }

        public EmailDomain(
            IEmailPersistence diskPersister,
            IEmailPersistence databasePersister,
            EmailServiceProxy emailServiceProxy)
        {
            this.emailServiceProxy = emailServiceProxy;
            this.diskPersister = diskPersister;
            this.databasePersister = databasePersister;
        }

        public Email LoadEmailFromDatabase(int id)
        {
            throw new NotImplementedException();
        }

        public void PersistEmailToDatabase(ref Email email)
        {
            this.databasePersister.PersistEmail(ref email);
        }

        public string PersistEmailToDisk(ref Email email)
        {
            return this.diskPersister.PersistEmail(ref email);
        }

        public long SendEmail(string context, ref Email email)
        {
            var emailMessage = Mapper.Map<EmailMessage>(email);
            emailMessage.Application = context;
            return this.emailServiceProxy.SendEmail(emailMessage);
        }

        public byte[] GetEmailMimeBytes(Email email)
        {
            return EmailUtilities.GetEmailBytes(email);
        }
    }
}