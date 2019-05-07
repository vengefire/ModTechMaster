namespace Framework.Logic.Email
{
    using System;
    using Domain.Email.Models;
    using Interfaces.Email;

    public class EmailDatabasePersistence : IEmailPersistence
    {
        public string PersistEmail(ref Email email)
        {
            throw new NotImplementedException();
        }

        public string PersistEmail(ref Email email, string uniqueId)
        {
            throw new NotImplementedException();
        }
    }
}