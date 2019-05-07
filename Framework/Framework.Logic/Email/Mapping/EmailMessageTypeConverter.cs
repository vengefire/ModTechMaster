namespace Framework.Logic.Email.Mapping
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Domain.Email.Models;
    using Microsoft.Exchange.WebServices.Data;

    public class EmailMessageTypeConverter : ITypeConverter<EmailMessage, Email>
    {
        static EmailMessageTypeConverter()
        {
            Mapper.CreateMap<EmailAddressCollection, List<string>>()
                  .ConvertUsing(
                                emailAddressCollection =>
                                    emailAddressCollection.Select(emailAddress => emailAddress.ToString()).ToList());

            Mapper.CreateMap<FileAttachment, EmailFileAttachment>().ConvertUsing<EmailFileAttachmentTypeConverter>();

            /*      .BeforeMap((src, dest) => src.Load())
                  .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                  .ForMember(dest => dest.Length, opt => opt.MapFrom(src => src.Size))
                  .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                  .ForMember(dest => dest.Id, opt => opt.UseValue(null));*/
            Mapper.CreateMap<AttachmentCollection, List<EmailFileAttachment>>()
                  .ConvertUsing(
                                attachments =>
                                    attachments.Where(attachment => attachment is FileAttachment)
                                               .Select(Mapper.Map<EmailFileAttachment>).ToList());
        }

        public Email Convert(ResolutionContext context)
        {
            var emailMessage = context.SourceValue as EmailMessage;
            return new Email
                   {
                       FromAddress = emailMessage.From.ToString(),
                       ToAddresses = Mapper.Map<List<string>>(emailMessage.ToRecipients),
                       CCAddresses = Mapper.Map<List<string>>(emailMessage.CcRecipients),
                       BCCAddresses = Mapper.Map<List<string>>(emailMessage.BccRecipients),
                       Subject = emailMessage.Subject,
                       Body = emailMessage.Body.ToString(),
                       DateReceived = emailMessage.DateTimeReceived,
                       FileAttachments = Mapper.Map<List<EmailFileAttachment>>(emailMessage.Attachments),
                       MimeBytes = emailMessage.MimeContent.Content
                   };
        }
    }
}