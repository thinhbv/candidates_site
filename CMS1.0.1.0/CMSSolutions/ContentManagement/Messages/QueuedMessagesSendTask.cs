using System;
using Castle.Core.Logging;
using CMSSolutions.ContentManagement.Messages.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Net.Mail;
using CMSSolutions.Tasks;

namespace CMSSolutions.ContentManagement.Messages
{
    [Feature(Constants.Areas.Messages)]
    public class QueuedMessagesSendTask : IScheduleTask
    {
        public QueuedMessagesSendTask()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public string Name { get { return "Gửi email tự động"; } }

        public bool Enabled { get { return false; } }

        public string CronExpression { get { return "0 0 23 ? * MON-FRI *"; } }

        public bool DisallowConcurrentExecution { get { return true; } }

        public void Execute(IWorkContextScope scope)
        {
            int maxTries;
            int messagesPerBatch;

            var smtpSettings = scope.Resolve<SmtpSettings>();
            if (string.IsNullOrEmpty(smtpSettings.Host))
            {
                maxTries = smtpSettings.MaxTries;
                messagesPerBatch = smtpSettings.MessagesPerBatch;
            }
            else
            {
                maxTries = 3;
                messagesPerBatch = 500;
            }

            var messageService = scope.Resolve<IMessageService>();
            var queuedEmails = messageService.GetQueuedEmails(maxTries, true, false, messagesPerBatch);

            if (queuedEmails.Count == 0)
            {
                return;
            }

            var emailSender = scope.Resolve<IEmailSender>();

            foreach (var queuedEmail in queuedEmails)
            {
                try
                {
                    var mailMessage = queuedEmail.GetMailMessage();
                    emailSender.Send(mailMessage);

                    queuedEmail.SentOnUtc = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    Logger.Error(string.Format("Error sending e-mail. {0}", exc.Message), exc);
                }
                finally
                {
                    queuedEmail.SentTries = queuedEmail.SentTries + 1;
                    messageService.Update(queuedEmail);
                }
            }
        }
    }
}