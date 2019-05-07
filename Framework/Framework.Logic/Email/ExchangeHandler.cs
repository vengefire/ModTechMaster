using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using Castle.Core.Internal;
using Castle.Core.Logging;
using Framework.Domain.Email.Models;
using Framework.Interfaces.Email;
using Framework.Logic.Email.Mapping;
using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

namespace Framework.Logic.Email
{
    public class ExchangeHandler : IExchangeHandler, IDisposable
    {
        private readonly string customDomain;

        private readonly string customPassword;

        private readonly string customUsername;
        private readonly object locker = new object();

        private readonly ILogger logger;

        private readonly string mailboxFolder;

        private readonly Timer pollTimer;

        private readonly bool useCustomCreds;

        private int consecutiveFailureCount;

        private ExchangeService exchangeService;

        private FolderId folderId;

        private HandlerStatus handlerStatus = HandlerStatus.Uninitialized;

        private string mailbox;

        private DateTime mailboxConnectionTimeStamp;

        private volatile bool monitorMailbox;

        private int numMessagesPerTick;

        private int pollTimeInMilliseconds = 10000;

        static ExchangeHandler()
        {
            Mapper.CreateMap<EmailMessage, Domain.Email.Models.Email>().ConvertUsing<EmailMessageTypeConverter>();
        }

        public ExchangeHandler(ILogger logger)
        {
            useCustomCreds = Convert.ToBoolean(ConfigurationManager.AppSettings["MailBox_UseCustomCreds"]);
            mailboxFolder = ConfigurationManager.AppSettings["MailBox_Folder"];

            if (useCustomCreds)
            {
                customUsername = ConfigurationManager.AppSettings["MailBox_Username"];
                customPassword = ConfigurationManager.AppSettings["MailBox_Password"];
                customDomain = ConfigurationManager.AppSettings["MailBox_Domain"];
            }

            this.logger = logger;
            pollTimer = new Timer(PollMailboxTick);
            logger.Debug("Exchange Handler constructed.");
        }

        public void Dispose()
        {
            // Ensure the monitoring closes off cleanly...
            if (HandlerStatus.Monitoring == handlerStatus)
            {
                StopMonitoring();
            }
        }

        public event MailboxMonitoringStartedEventHandler MailboxMonitoringStartedEventHandler;

        public event MailboxMonitoringStoppedEventHandler MailboxMonitoringStoppedEventHandler;

        public event ProcessEmailEventHandler ProcessEmailEventHandler;

        public void ConnectToMailbox(string mailbox)
        {
            if (HandlerStatus.Uninitialized != handlerStatus)
            {
                throw new InvalidProgramException("The ExchangeMonitor has already been connected.");
            }

            this.mailbox = mailbox;

            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
            exchangeService = new ExchangeService(ExchangeVersion.Exchange2010)
            {
                Credentials =
                    useCustomCreds
                        ? new NetworkCredential(customUsername, customPassword, customDomain)
                        : new NetworkCredential(),
                UseDefaultCredentials = !useCustomCreds,
                KeepAlive = true,
                Timeout = int.MaxValue
            };

            logger.InfoFormat("Connecting to mailbox [{0}]...", mailbox);
            exchangeService.AutodiscoverUrl(mailbox, RedirectionUrlValidationCallback);
            if (mailboxFolder.IsNullOrEmpty())
            {
                folderId = new FolderId(WellKnownFolderName.Inbox, new Mailbox(mailbox));
            }
            else
            {
                var folderView = new FolderView(100)
                {
                    PropertySet = new PropertySet(BasePropertySet.IdOnly)
                    {
                        FolderSchema.DisplayName
                    },
                    Traversal = FolderTraversal.Shallow
                };

                var findFolder = new Func<FolderId, string, FolderId>(
                    (parentFolder, searchFolderName) =>
                    {
                        var results = exchangeService.FindFolders(parentFolder, folderView);
                        return results.First(folder => folder.DisplayName.Equals(searchFolderName)).Id;
                    });

                var folderPaths = mailboxFolder.Replace("\\", "/").Split(
                    new[]
                    {
                        "/"
                    },
                    StringSplitOptions.RemoveEmptyEntries);
                var recurseFolderId = new FolderId(WellKnownFolderName.MsgFolderRoot, new Mailbox(mailbox));
                recurseFolderId = folderPaths.Aggregate(recurseFolderId,
                    (current, folderPath) => findFolder(current, folderPath));
                folderId = recurseFolderId;
            }

            logger.InfoFormat("Folder ID set as [{0} - {1}].", this.mailbox, mailboxFolder);
            handlerStatus = HandlerStatus.Idle;
            mailboxConnectionTimeStamp = DateTime.Now;
        }

        public void StartMonitoring(int pollTimeMilliseconds, int numMessagesPerTick)
        {
            logger.Info("Starting monitoring of the mailbox, validating arguments...");
            if (1 > numMessagesPerTick)
            {
                throw new ArgumentException("numMessagesPerTick must exceed 0.");
            }

            if (1 > pollTimeMilliseconds)
            {
                throw new ArgumentException("pollTimeMilliseconds must exceed 0.");
            }

            if (HandlerStatus.Idle != handlerStatus)
            {
                throw new InvalidProgramException("The ExchangeMonitor is not Idle.");
            }

            if (null == ProcessEmailEventHandler)
            {
                throw new ArgumentException("A ProcessEmailEventHandler must be set.");
            }

            handlerStatus = HandlerStatus.Monitoring;
            this.numMessagesPerTick = numMessagesPerTick;
            monitorMailbox = true;
            StartTimer(pollTimeMilliseconds);

            logger.Info("Monitoring of the mailbox has commenced...");
            if (null != MailboxMonitoringStartedEventHandler)
            {
                MailboxMonitoringStartedEventHandler();
            }
        }

        public void StopMonitoring()
        {
            logger.Info("Stopping monitoring of the mailbox ...");
            StopTimer();

            lock (locker)
            {
                if (HandlerStatus.Monitoring != handlerStatus)
                {
                    throw new InvalidProgramException("The ExchangeMonitor is not currently monitoring.");
                }

                monitorMailbox = false;
                handlerStatus = HandlerStatus.Idle;
                logger.Debug("Mailbox monitoring has halted.");
                if (null != MailboxMonitoringStoppedEventHandler)
                {
                    MailboxMonitoringStoppedEventHandler();
                }
            }
        }

        private static List<string> ConvertEmailAddressCollection(EmailAddressCollection emailAddressCollection)
        {
            return emailAddressCollection.Select(address => address.ToString()).ToList();
        }

        private static async Task<List<EmailFileAttachment>> ConvertAttachmentCollection(
            AttachmentCollection attachmenCollection)
        {
            var emailFileAttachments = new EditableList<EmailFileAttachment>();

            var fileAttachments = attachmenCollection.Where(attachment => attachment is FileAttachment);
            foreach (var fileAttachment in fileAttachments)
            {
                var emailFileAttachment = await ConvertFileAttachment(fileAttachment as FileAttachment);
                emailFileAttachments.Add(emailFileAttachment);
            }

            return emailFileAttachments;

            // return attachmenCollection.Where(attachment => attachment is FileAttachment).Select(async attachment => await ExchangeHandler.ConvertFileAttachment(attachment as FileAttachment)).ToList();
        }

        private static async Task<EmailFileAttachment> ConvertFileAttachment(FileAttachment attachment)
        {
            await Task.Factory.StartNew(attachment.Load);

            return new EmailFileAttachment
            {
                Name = attachment.Name,
                Length = attachment.Size,
                Content = attachment.Content
            };
        }

        private async void PollMailboxTick(object state)
        {
            StopTimer();
            if (false == monitorMailbox)
            {
                return;
            }

            if ((DateTime.Now - mailboxConnectionTimeStamp).Hours >= 3 || consecutiveFailureCount != 0)
            {
                try
                {
                    RefreshMailBoxConnection();
                }
                catch (Exception ex)
                {
                    consecutiveFailureCount += 1;
                    logger.ErrorFormat(ex,
                        "An Exception was encountered while refreshing the mailbox connection. Consecutive Failure Count = [{0}]...",
                        consecutiveFailureCount);
                    if (consecutiveFailureCount > 10)
                    {
                        logger.ErrorFormat(ex, "Consecutive Failure Count exceeded 10, re-throwing exception...");
                        throw;
                    }
                    if (monitorMailbox)
                    {
                        StartTimer(pollTimeInMilliseconds);
                    }

                    return;
                }
            }

            logger.Debug("Processing Mailbox tick...");

            // Process emails until the mailbox is empty...
            EmailMessage[] emails = null;
            var getEmailSuccess = GetEmails(out emails);

            while (getEmailSuccess && emails.Any())
            {
                var tasks = new Task[emails.Count()];
                for (var index = 0; index < emails.Count(); index++)
                {
                    var emailItem = emails[index];
                    tasks[index] = ProcessEmail(emailItem);
                }

                await Task.WhenAll(tasks);
                getEmailSuccess = GetEmails(out emails);
            }

            logger.Debug("Mailbox tick processing complete.");
            if (monitorMailbox)
            {
                StartTimer(pollTimeInMilliseconds);
            }
        }

        private async Task<Domain.Email.Models.Email> ConvertEmailMessage(EmailMessage emailMessage)
        {
            logger.DebugFormat("Converting email message [{0}] to email...", emailMessage.Subject);

            var email = new Domain.Email.Models.Email
            {
                FromAddress = emailMessage.From.ToString(),
                ToAddresses = ConvertEmailAddressCollection(emailMessage.ToRecipients),
                CCAddresses = ConvertEmailAddressCollection(emailMessage.CcRecipients),
                BCCAddresses = ConvertEmailAddressCollection(emailMessage.BccRecipients),
                Subject = emailMessage.Subject,
                Body = emailMessage.Body,
                DateReceived = DateTime.Now,
                FileAttachments = await ConvertAttachmentCollection(emailMessage.Attachments),
                MimeBytes = emailMessage.MimeContent.Content
            };

            logger.DebugFormat("Email message [{0}] converted to email.", emailMessage.Subject);
            return email;
        }

        private async void ProcessEmailItem(Item emailItem)
        {
            if (false == monitorMailbox)
            {
                logger.Info("Monitoring has been stopped, exiting message processing...");
                return;
            }

            if (null != ProcessEmailEventHandler)
            {
                try
                {
                    var emailMessage = TransformFindResult(emailItem);

                    logger.InfoFormat(
                        "Delegating email message with subject [{0}] processing to client...",
                        emailMessage.Subject);
                    var email = await ConvertEmailMessage(emailMessage);

                    var deleteEmail = ProcessEmailEventHandler(email);

                    if (deleteEmail)
                    {
                        emailMessage.Delete(DeleteMode.SoftDelete);
                        logger.DebugFormat("Processed email message [{0}] deleted.", emailMessage.Subject);
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat(ex,
                        "An exception occurred processing item [{0}]. Item has been left in the mail box",
                        emailItem.Subject);
                }
            }
            else
            {
                throw new InvalidProgramException("The ProcessEmailEventHandler has been rendered null.");
            }
        }

        private async Task ProcessEmail(EmailMessage emailMessage)
        {
            if (false == monitorMailbox)
            {
                logger.Info("Monitoring has been stopped, exiting message processing...");
                return;
            }

            if (null != ProcessEmailEventHandler)
            {
                try
                {
                    logger.InfoFormat(
                        "Delegating email message with subject [{0}] processing to client...",
                        emailMessage.Subject);

                    var email = await ConvertEmailMessage(emailMessage);

                    var deleteEmail = ProcessEmailEventHandler(email);

                    if (deleteEmail)
                    {
                        emailMessage.Delete(DeleteMode.SoftDelete);
                        logger.DebugFormat("Processed email message [{0}] deleted.", emailMessage.Subject);
                    }
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat(ex,
                        "An exception occurred processing item [{0}]. Item has been left in the mail box",
                        emailMessage.Subject);
                }
            }
            else
            {
                throw new InvalidProgramException("The ProcessEmailEventHandler has been rendered null.");
            }
        }

        private void RefreshMailBoxConnection()
        {
            try
            {
                logger.InfoFormat(
                    "Refreshing mailbox connection, last refreshed at [{0}]...",
                    mailboxConnectionTimeStamp);

                if (HandlerStatus.Monitoring == handlerStatus)
                {
                    StopMonitoring();
                }

                handlerStatus = HandlerStatus.Uninitialized;
                ConnectToMailbox(mailbox);
                handlerStatus = HandlerStatus.Monitoring;
            }
            finally
            {
                monitorMailbox = true;
            }
        }

        private bool CertificateValidationCallBack(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (var status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer)
                            && (status.Status == X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                        }
                        else
                        {
                            if (status.Status != X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }

            // In all other cases, return false.
            return false;
        }

        private FindItemsResults<Item> GetEmailItems()
        {
            logger.DebugFormat(
                "Fetching up to [{0}] email(s) from [{1}]...",
                numMessagesPerTick,
                folderId.ToString());
            var findResults = exchangeService.FindItems(
                folderId,
                new ItemView(numMessagesPerTick));

            logger.DebugFormat("FindItems retrieved [{0}] entries...", findResults.Count());

            return findResults;
        }

        private bool GetEmails(out EmailMessage[] emails)
        {
            emails = null;
            try
            {
                logger.DebugFormat(
                    "Fetching up to [{0}] email(s) from [{1}]...",
                    numMessagesPerTick,
                    folderId.ToString());

                var findResults = exchangeService.FindItems(
                    folderId,
                    new ItemView(numMessagesPerTick));

                logger.DebugFormat("FindItems retrieved [{0}] entries...", findResults.Count());

                emails = findResults.Select(TransformFindResult).ToArray();
            }
            catch (Exception ex)
            {
                consecutiveFailureCount += 1;
                logger.ErrorFormat(ex,
                    "An Exception was encountered while retrieving the Email Messages. Consecutive Failure Count = [{0}]...",
                    consecutiveFailureCount);
                if (consecutiveFailureCount > 10)
                {
                    logger.ErrorFormat(ex, "Consecutive Failure Count exceeded 10, re-throwing exception...");
                    throw;
                }

                logger.WarnFormat("Consecutive Failure Count < 10, allowing process to roll over to next tick.");
                return false;
            }

            consecutiveFailureCount = 0;

            return true;
        }

        private EmailMessage TransformFindResult(Item item)
        {
            logger.DebugFormat("Running transform on Item [{0}]...", item.Subject);
            var result = EmailMessage.Bind(
                exchangeService,
                item.Id,
                new PropertySet(
                    BasePropertySet.FirstClassProperties,
                    ItemSchema.Attachments,
                    ItemSchema.MimeContent));
            logger.DebugFormat("Item [{0}] transformed.", item.Subject);
            return result;
        }

        private bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            var result = false;

            var redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }

            return result;
        }

        private void StartTimer(int polltimeInMilliseconds)
        {
            pollTimeInMilliseconds = polltimeInMilliseconds;
            pollTimer.Change(pollTimeInMilliseconds, Timeout.Infinite);
        }

        private void StopTimer()
        {
            pollTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        ////public event Func<Email, Task<bool>> ProcessEmailEventHandlerAsync;

        private enum HandlerStatus
        {
            Uninitialized,

            Monitoring,

            Idle
        }
    }
}