namespace Framework.Logic.Email.Mapping
{
    using AutoMapper;
    using Domain.Email.Models;
    using Microsoft.Exchange.WebServices.Data;

    public class EmailFileAttachmentTypeConverter : ITypeConverter<FileAttachment, EmailFileAttachment>
    {
        public EmailFileAttachment Convert(ResolutionContext context)
        {
            var fileAttachment = context.SourceValue as FileAttachment;
            fileAttachment.Load();
            return new EmailFileAttachment {Name = fileAttachment.Name, Length = fileAttachment.Size, Content = fileAttachment.Content};
        }
    }
}