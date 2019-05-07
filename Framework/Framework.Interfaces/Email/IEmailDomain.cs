namespace Framework.Interfaces.Email
{
    using Domain.Email.Models;

    public interface IEmailDomain
    {
        byte[] GetEmailMimeBytes(Email email);

        Email LoadEmailFromDatabase(int id);

        void PersistEmailToDatabase(ref Email email);

        string PersistEmailToDisk(ref Email email);

        long SendEmail(string context, ref Email email);
    }
}