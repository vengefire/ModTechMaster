namespace Framework.Interfaces.Email
{
    public interface IEmailDomain
    {
        byte[] GetEmailMimeBytes(Domain.Email.Models.Email email);

        Domain.Email.Models.Email LoadEmailFromDatabase(int id);

        void PersistEmailToDatabase(ref Domain.Email.Models.Email email);

        string PersistEmailToDisk(ref Domain.Email.Models.Email email);

        long SendEmail(string context, ref Domain.Email.Models.Email email);
    }
}