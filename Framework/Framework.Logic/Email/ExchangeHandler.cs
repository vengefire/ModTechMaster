namespace Framework.Logic.Email
{
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
    using Domain.Email.Models;
    using Interfaces.Email;
    using Mapping;
    using Microsoft.Exchange.WebServices.Data;
    using Task = System.Threading.Tasks.Task;

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
            Mapper.CreateMap<EmailMessage, Email>().ConvertUsing<EmailMessageTypeConverter>();
        }

        public ExchangeHandler(ILogger logger)
        {
            this.useCustomCreds = Convert.ToBoolean(ConfigurationManager.AppSettings["MailBox_UseCustomCreds"]);
            this.mailboxFolder = ConfigurationManager.AppSettings["MailBox_Folder"];

            if (this.useCustomCreds)
            {
                this.customUsername = ConfigurationManager.AppSettings["MailBox_Username"];
                this.customPassword = ConfigurationManager.AppSettings["MailBox_Password"];
                this.customDomain = ConfigurationManager.AppSettings["MailBox_Domain"];
            }

            this.logger = logger;
            this.pollTimer = new Timer(this.PollMailboxTick);
            logger.Debug("Exchange Handler constructed.");
        }

        public void Dispose()
        {
            // Ensure the monitoring closes off cleanly...
            if (HandlerStatus.Monitoring == this.handlerStatus)
            {
                this.StopMonitoring();
            }
        }

        public event MailboxMonitoringStartedEventHandler MailboxMonitoringStartedEventHandler;

        public event MailboxMonitoringStoppedEventHandler MailboxMonitoringStoppedEventHandler;

        public event ProcessEmailEventHandler ProcessEmailEventHandler;

        public void ConnectToMailbox(string mailbox)
        {
            if (HandlerStatus.Uninitialized != this.handlerStatus)
            {
                throw new InvalidProgramException("The ExchangeMonitor has already been connected.");
            }

            this.mailbox = mailbox;

            ServicePointManager.ServerCertificateValidationCallback = this.CertificateValidationCallBack;
            this.exchangeService = new ExchangeService(ExchangeVersion.Exchange2010)
                                   {
                                       Credentials = this.useCustomCreds
                                           ? new NetworkCredential(this.customUsername, this.customPassword, this.customDomain)
                                           : new NetworkCredential(),
                                       UseDefaultCredentials = !this.useCustomCreds,
                                       KeepAlive = true,
                                       Timeout = int.MaxValue
                                   };

            this.logger.InfoFormat("Connecting to mailbox [{0}]...", mailbox);
            this.exchangeService.AutodiscoverUrl(mailbox, this.RedirectionUrlValidationCallback);
            if (this.mailboxFolder.IsNullOrEmpty())
            {
                this.folderId = new FolderId(WellKnownFolderName.Inbox, new Mailbox(mailbox));
            }
            else
            {
                var folderView = new FolderView(100) {PropertySet = new PropertySet(BasePropertySet.IdOnly) {FolderSchema.DisplayName}, Traversal = FolderTraversal.Shallow};

                var findFolder = new Func<FolderId, string, FolderId>(
                                                                      (parentFolder, searchFolderName) =>
                                                                      {
                                                                          var results = this.exchangeService.FindFolders(parentFolder, folderView);
                                                                          return results.First(folder => folder.DisplayName.Equals(searchFolderName)).Id;
                                                                      });

                var folderPaths = this.mailboxFolder.Replace("\\", "/").Split(
                                                                              new[] {"/"},
                                                                              StringSplitOptions.RemoveEmptyEntries);
                var recurseFolderId = new FolderId(WellKnownFolderName.MsgFolderRoot, new Mailbox(mailbox));
                recurseFolderId = folderPaths.Aggregate(
                                                        recurseFolderId,
                                                        (current, folderPath) => findFolder(current, folderPath));
                this.folderId = recurseFolderId;
            }

            this.logger.InfoFormat("Folder ID set as [{0} - {1}].", this.mailbox, this.mailboxFolder);
            this.handlerStatus = HandlerStatus.Idle;
            this.mailboxConnectionTimeStamp = DateTime.Now;
        }

        public void StartMonitoring(int pollTimeMilliseconds, int numMessagesPerTick)
        {
            this.logger.Info("Starting monitoring of the mailbox, validating arguments...");
            if (1 > numMessagesPerTick)
            {
                throw new ArgumentException("numMessagesPerTick must exceed 0.");
            }

            if (1 > pollTimeMilliseconds)
            {
                throw new ArgumentException("pollTimeMilliseconds must exceed 0.");
            }

            if (HandlerStatus.Idle != this.handlerStatus)
            {
                throw new InvalidProgramException("The ExchangeMonitor is not Idle.");
            }

            if (null == this.ProcessEmailEventHandler)
            {
                throw new ArgumentException("A ProcessEmailEventHandler must be set.");
            }

            this.handlerStatus = HandlerStatus.Monitoring;
            this.numMessagesPerTick = numMessagesPerTick;
            this.monitorMailbox = true;
            this.StartTimer(pollTimeMilliseconds);

            this.logger.Info("Monitoring of the mailbox has commenced...");
            if (null != this.MailboxMonitoringStartedEventHandler)
            {
                this.MailboxMonitoringStartedEventHandler();
            }
        }

        public void StopMonitoring()
        {
            this.logger.Info("Stopping monitoring of the mailbox ...");
            this.StopTimer();

            lock (this.locker)
            {
                if (HandlerStatus.Monitoring != this.handlerStatus)
                {
                    throw new InvalidProgramException("The ExchangeMonitor is not currently monitoring.");
                }

                this.monitorMailbox = false;
                this.handlerStatus = HandlerStatus.Idle;
                this.logger.Debug("Mailbox monitoring has halted.");
                if (null != this.MailboxMonitoringStoppedEventHandler)
                {
                    this.MailboxMonitoringStoppedEventHandler();
                }
            }
        }

        private static List<string> ConvertEmailAddressCollection(EmailAddressCollection emailAddressCollection)
        {
            return emailAddressCollection.Select(address => address.ToString()).ToList();
        }

        private static async Task<List<EmailFileAttachment>> ConvertAttachmentCollection(AttachmentCollection attachmenCollection)
        {
            var emailFileAttachments = new EditableList<EmailFileAttachment>();

            var fileAttachments = attachmenCollection.Where(attachment => attachment is FileAttachment);
            foreach (var fileAttachment in fileAttachments)
            {
                var emailFileAttachment = await ExchangeHandler.ConvertFileAttachment(fileAttachment as FileAttachment);
                emailFileAttachments.Add(emailFileAttachment);
            }

            return emailFileAttachments;

            // return attachmenCollection.Where(attachment => attachment is FileAttachment).Select(async attachment => await ExchangeHandler.ConvertFileAttachment(attachment as FileAttachment)).ToList();
        }

        private static async Task<EmailFileAttachment> ConvertFileAttachment(FileAttachment attachment)
        {
            await Task.Factory.StartNew(attachment.Load);

            return new EmailFileAttachment {Name = attachment.Name, Length = attachment.Size, Content = attachment.Content};
        }

        private async void PollMailboxTick(object state)
        {
            this.StopTimer();
            if (false == this.monitorMailbox)
            {
                return;
            }

            if ((DateTime.Now - this.mailboxConnectionTimeStamp).Hours >= 3 ||
                this.consecutiveFailureCount != 0)
            {
                try
                {
                    this.RefreshMailBoxConnection();
                }
                catch (Exception ex)
                {
                    this.consecutiveFailureCount += 1;
                    this.logger.ErrorFormat(
                                            ex,
                                            "An Exception was encountered while refreshing the mailbox connection. Consecutive Failure Count = [{0}]...",
                                            this.consecutiveFailureCount);
                    if (this.consecutiveFailureCount > 10)
                    {
                        this.logger.ErrorFormat(ex, "Consecutive Failure Count exceeded 10, re-throwing exception...");
                        throw;
                    }

                    if (this.monitorMailbox)
                    {
                        this.StartTimer(this.pollTimeInMilliseconds);
                    }

                    return;
                }
            }

            this.logger.Debug("Processing Mailbox tick...");

            // Process emails until the mailbox is empty...
            EmailMessage[] emails = null;
            var getEmailSuccess = this.GetEmails(out emails);

            while (getEmailSuccess && emails.Any())
            {
                var tasks = new Task[emails.Count()];
                for (var index = 0; index < emails.Count(); index++)
                {
                    var emailItem = emails[index];
                    tasks[index] = this.ProcessEmail(emailItem);
                }

                await Task.WhenAll(tasks);
                getEmailSuccess = this.GetEmails(out emails);
            }

            this.logger.Debug("Mailbox tick processing complete.");
            if (this.monitorMailbox)
            {
                this.StartTimer(this.pollTimeInMilliseconds);
            }
        }

        private async Task<Email> ConvertEmailMessage(EmailMessage emailMessage)
        {
            this.logger.DebugFormat("Converting email message [{0}] to email...", emailMessage.Subject);

            var email = new Email
                        {
                            FromAddress = emailMessage.From.ToString(),
                            ToAddresses = ExchangeHandler.ConvertEmailAddressCollection(emailMessage.ToRecipients),
                            CCAddresses = ExchangeHandler.ConvertEmailAddressCollection(emailMessage.CcRecipients),
                            BCCAddresses = ExchangeHandler.ConvertEmailAddressCollection(emailMessage.BccRecipients),
                            Subject = emailMessage.Subject,
                            Body = emailMessage.Body,
                            DateReceived = DateTime.Now,
                            FileAttachments = await ExchangeHandler.ConvertAttachmentCollection(emailMessage.Attachments),
                            MimeBytes = emailMessage.MimeContent.Content
                        };

            this.logger.DebugFormat("Email message [{0}] converted to email.", emailMessage.Subject);
            return email;
        }

        private async void ProcessEmailItem(Item emailItem)
        {
            if (false == this.monitorMailbox)
            {
                this.logger.Info("Monitoring has been stopped, exiting message processing...");
                return;
            }

            if (null != this.ProcessEmailEventHandler)
            {
                try
                {
                    var emailMessage = this.TransformFindResult(emailItem);

                    this.logger.InfoFormat(
                                           "Delegating email message with subject [{0}] processing to client...",
                                           emailMessage.Subject);
                    var email = await this.ConvertEmailMessage(emailMessage);

                    var deleteEmail = this.ProcessEmailEventHandler(email);

                    if (deleteEmail)
                    {
                        emailMessage.Delete(DeleteMode.SoftDelete);
                        this.logger.DebugFormat("Processed email message [{0}] deleted.", emailMessage.Subject);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.ErrorFormat(
                                            ex,
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
            if (false == this.monitorMailbox)
            {
                this.logger.Info("Monitoring has been stopped, exiting message processing...");
                return;
            }

            if (null != this.ProcessEmailEventHandler)
            {
                try
                {
                    this.logger.InfoFormat(
                                           "Delegating email message with subject [{0}] processing to client...",
                                           emailMessage.Subject);

                    var email = await this.ConvertEmailMessage(emailMessage);

                    var deleteEmail = this.ProcessEmailEventHandler(email);

                    if (deleteEmail)
                    {
                        emailMessage.Delete(DeleteMode.SoftDelete);
                        this.logger.DebugFormat("Processed email message [{0}] deleted.", emailMessage.Subject);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.ErrorFormat(
                                            ex,
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
                this.logger.InfoFormat(
                                       "Refreshing mailbox connection, last refreshed at [{0}]...",
                                       this.mailboxConnectionTimeStamp);

                if (HandlerStatus.Monitoring == this.handlerStatus)
                {
                    this.StopMonitoring();
                }

                this.handlerStatus = HandlerStatus.Uninitialized;
                this.ConnectToMailbox(this.mailbox);
                this.handlerStatus = HandlerStatus.Monitoring;
            }
            finally
            {
                this.monitorMailbox = true;
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
                if (chain != null &&
                    chain.ChainStatus != null)
                {
                    foreach (var status in chain.ChainStatus)
                        if (certificate.Subject == certificate.Issuer &&
                            status.Status == X509ChainStatusFlags.UntrustedRoot)
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
            this.logger.DebugFormat(
                                    "Fetching up to [{0}] email(s) from [{1}]...",
                                    this.numMessagesPerTick,
                                    this.folderId.ToString());
            var findResults = this.exchangeService.FindItems(
                                                             this.folderId,
                                                             new ItemView(this.numMessagesPerTick));

            this.logger.DebugFormat("FindItems retrieved [{0}] entries...", findResults.Count());

            return findResults;
        }

        private bool GetEmails(out EmailMessage[] emails)
        {
            emails = null;
            try
            {
                this.logger.DebugFormat(
                                        "Fetching up to [{0}] email(s) from [{1}]...",
                                        this.numMessagesPerTick,
                                        this.folderId.ToString());

                var findResults = this.exchangeService.FindItems(
                                                                 this.folderId,
                                                                 new ItemView(this.numMessagesPerTick));

                this.logger.DebugFormat("FindItems retrieved [{0}] entries...", findResults.Count());

                emails = findResults.Select(this.TransformFindResult).ToArray();
            }
            catch (Exception ex)
            {
                this.consecutiveFailureCount += 1;
                this.logger.ErrorFormat(
                                        ex,
                                        "An Exception was encountered while retrieving the Email Messages. Consecutive Failure Count = [{0}]...",
                                        this.consecutiveFailureCount);
                if (this.consecutiveFailureCount > 10)
                {
                    this.logger.ErrorFormat(ex, "Consecutive Failure Count exceeded 10, re-throwing exception...");
                    throw;
                }

                this.logger.WarnFormat("Consecutive Failure Count < 10, allowing process to roll over to next tick.");
                return false;
            }

            this.consecutiveFailureCount = 0;

            return true;
        }

        private EmailMessage TransformFindResult(Item item)
        {
            this.logger.DebugFormat("Running transform on Item [{0}]...", item.Subject);
            var result = EmailMessage.Bind(
                                           this.exchangeService,
                                           item.Id,
                                           new PropertySet(
                                                           BasePropertySet.FirstClassProperties,
                                                           ItemSchema.Attachments,
                                                           ItemSchema.MimeContent));
            this.logger.DebugFormat("Item [{0}] transformed.", item.Subject);
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
            this.pollTimeInMilliseconds = polltimeInMilliseconds;
            this.pollTimer.Change(this.pollTimeInMilliseconds, Timeout.Infinite);
        }

        private void StopTimer()
        {
            this.pollTimer.Change(Timeout.Infinite, Timeout.Infinite);
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