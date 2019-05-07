using System;
using Framework.Interfaces.Email;

namespace Framework.Logic.Email
{
    public class EmailDatabasePersistence : IEmailPersistence
    {
        public string PersistEmail(ref Domain.Email.Models.Email email)
        {
            throw new NotImplementedException();
        }

        public string PersistEmail(ref Domain.Email.Models.Email email, string uniqueId)
        {
            throw new NotImplementedException();
        }
    }
}