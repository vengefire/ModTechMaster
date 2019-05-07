namespace Framework.Domain.Email.Models
{
    public class EmailFileAttachment
    {
        public byte[] Content { get; set; }

        public Email Email { get; set; }

        public int? Id { get; set; }

        public int Length { get; set; }

        public string Name { get; set; }
    }
}