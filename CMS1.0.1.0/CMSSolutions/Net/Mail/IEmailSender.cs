using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using Castle.Core.Logging;

namespace CMSSolutions.Net.Mail
{
    public interface IEmailSender : IDependency
    {
        void Send(MailMessage mailMessage);

        void Send(string subject, string body, string toEmailAddress);
    }

    public class DefaultEmailSender : IEmailSender
    {
        private readonly SmtpSettings smtpSettings;

        public DefaultEmailSender(SmtpSettings smtpSettings)
        {
            this.smtpSettings = smtpSettings;
        }

        public ILogger Logger { get; set; }

        public void Send(MailMessage mailMessage)
        {
            using (var smtpClient = new SmtpClient())
            {
                if (smtpSettings != null && !string.IsNullOrEmpty(smtpSettings.Host))
                {
                    smtpClient.UseDefaultCredentials = smtpSettings.UseDefaultCredentials;
                    smtpClient.Host = smtpSettings.Host;
                    smtpClient.Port = smtpSettings.Port;
                    smtpClient.EnableSsl = smtpSettings.EnableSsl;
                    smtpClient.Credentials = smtpSettings.UseDefaultCredentials
                        ? CredentialCache.DefaultNetworkCredentials
                        : new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                    mailMessage.From = new MailAddress(smtpSettings.FromAddress, smtpSettings.DisplayName);
                }

                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    Logger.InfoFormat(ex, "Cannot send email message.");
                    throw ex;
                }
            }
        }

        public void Send(string subject, string body, string toEmailAddress)
        {
            var mailMessage = new MailMessage
                              {
                                  Subject = subject,
                                  SubjectEncoding = Encoding.UTF8,
                                  Body = body,
                                  BodyEncoding = Encoding.UTF8,
                                  IsBodyHtml = true
                              };
            mailMessage.To.Add(toEmailAddress);
            Send(mailMessage);
        }
    }
}
