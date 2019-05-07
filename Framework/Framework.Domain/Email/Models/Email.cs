namespace Framework.Domain.Email.Models
{
    using System;
    using System.Collections.Generic;

    public class Email
    {
        public List<string> BCCAddresses { get; set; }

        public string Body { get; set; }

        public List<string> CCAddresses { get; set; }

        public DateTime? DateReceived { get; set; }

        public DateTime? DateSent { get; set; }

        public List<EmailFileAttachment> FileAttachments { get; set; }

        public string FileName { get; set; }

        public string FromAddress { get; set; }

        public int? Id { get; set; }

        public bool IsContentHtml { get; set; }

        public byte[] MimeBytes { get; set; }

        public string PhysicalPath { get; set; }

        public string Subject { get; set; }

        public List<string> ToAddresses { get; set; }
    }
}