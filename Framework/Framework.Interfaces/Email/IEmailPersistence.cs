namespace Framework.Interfaces.Email
{
    public interface IEmailPersistence
    {
        /// <summary>
        ///     Persists an email
        /// </summary>
        /// <param name="email">The email to persist</param>
        /// <returns>A unique identifier for the persisted email</returns>
        string PersistEmail(ref Domain.Email.Models.Email email);

        /// <summary>
        ///     Persists an email with a specified unique identifier
        /// </summary>
        /// <param name="email">The email to perist</param>
        /// <param name="uniqueId">The unique identifier to use when persisting the email</param>
        /// <returns>The unique identifier for the persisted email</returns>
        string PersistEmail(ref Domain.Email.Models.Email email, string uniqueId);
    }
}