namespace Framework.Logic.Email
{
    using System;
    using System.IO;
    using System.Linq;
    using Domain.Email.Models;
    using Interfaces.Email;
    using Utils.Directory;

    public class EmailDiskPersistence : EmailBasePersistence, IEmailPersistence
    {
        private readonly string repositoryBaseDirectory;

        public EmailDiskPersistence(string repositoryBaseDirectory)
        {
            this.repositoryBaseDirectory = repositoryBaseDirectory;
        }

        public string PersistEmail(ref Email email, string uniqueId)
        {
            if (null == this.repositoryBaseDirectory)
            {
                throw new InvalidProgramException("No Email repository base directory setting could be found in the application configuration file.");
            }

            var repositoryRelativeDirectory = string.Format(
                                                            "{0}\\{1}\\{2}",
                                                            DateTime.Today.Year,
                                                            DateTime.Today.Month,
                                                            DateTime.Today.Day);
            var repositoryFinalDirectory = Path.Combine(this.repositoryBaseDirectory, repositoryRelativeDirectory);
            DirectoryUtils.EnsureExists(repositoryFinalDirectory);

            var filename = string.Format("{0}.eml", uniqueId);
            foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
                if (filename.Contains(invalidCharacter))
                {
                    filename = filename.Replace(invalidCharacter, '_');
                }

            var repositoryAbsolutePath = Path.Combine(repositoryFinalDirectory, filename);

            using (var fileStream = new FileStream(repositoryAbsolutePath, FileMode.Create))
            {
                if (email.MimeBytes == null)
                {
                    email.MimeBytes = EmailUtilities.GetEmailBytes(email);
                }

                fileStream.Write(email.MimeBytes, 0, email.MimeBytes.Length);
            }

            email.FileName = filename;
            email.PhysicalPath = repositoryFinalDirectory;

            return uniqueId;
        }

        public string PersistEmail(ref Email email)
        {
            return this.PersistEmail(ref email, Guid.NewGuid().ToString());
        }
    }
}