using System.IO;
using System.Net.Mail;
using System.Reflection;

namespace Framework.Utils.Extensions.MailMessage
{
    public static class MailMessageExtensions
    {
        public static byte[] GetEmailContentBytes(this System.Net.Mail.MailMessage message)
        {
            var assembly = typeof(SmtpClient).Assembly;
            var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

            using (var stream = new MemoryStream())
            {
                // .Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture, Boolean skipVisibilityChecks)
                // Get reflection info for MailWriter constructor
                var mailWriterContructor = mailWriterType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new[] {typeof(Stream)},
                    null);

                // Construct MailWriter object with our FileStream
                var mailWriter = mailWriterContructor.Invoke(
                    new object[]
                    {
                        stream
                    });

                // Get reflection info for Send() method on MailMessage
                var sendMethod = typeof(System.Net.Mail.MailMessage).GetMethod("Send",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                // Call method passing in MailWriter
                if (sendMethod.GetParameters().Length == 2)
                {
                    sendMethod.Invoke(
                        message,
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new[] {mailWriter, true},
                        null);
                }
                else
                {
                    sendMethod.Invoke(
                        message,
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new[] {mailWriter, true, true},
                        null);
                }

                /*sendMethod.Invoke(
                    message, 
                    BindingFlags.Instance | BindingFlags.NonPublic, 
                    null, 
                    new[] { mailWriter, true, true }, 
                    null);*/

                // Finally get reflection info for Close() method on our MailWriter
                var closeMethod = mailWriter.GetType()
                    .GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                closeMethod.Invoke(
                    mailWriter,
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new object[] {},
                    null);

                return stream.ToArray();
            }
        }
    }
}