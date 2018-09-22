using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using CMSSolutions.ContentManagement.Messages.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Messages.Services
{
    public interface IMessageService : IGenericService<QueuedEmail, Guid>, IDependency
    {
        Guid SendEmailMessage(string messageTemplate, IList<Token> tokens, string toEmailAddress, string toName = null);

        Guid SendEmailMessage(string subject, string body, string toEmailAddress, string toName = null);

        Guid SendEmailMessage(MailMessage mailMessage);

        Guid SendSmsMessage(string messageTemplate, IList<Token> tokens, string fromNumber, string toNumber);

        Guid SendSmsMessage(string fromNumber, string toNumber, string message);

        IList<QueuedEmail> GetQueuedEmails(int maxSendTries, bool loadNotSentItemsOnly, bool loadNewest, int pageSize);

        IList<QueuedSms> GetQueuedSms(int maxSendTries, bool loadNotSentItemsOnly, bool loadNewest, int pageSize);

        void Update(QueuedSms smsMessage);
    }

    [Feature(Constants.Areas.Messages)]
    public class MessageService : GenericService<QueuedEmail, Guid>, IMessageService
    {
        private readonly IMessageTemplateService messageTemplateService;
        private readonly ITokenizer tokenizer;
        private readonly IEnumerable<IMessageTokensProvider> tokenProviders;
        private readonly IWorkContextAccessor workContextAccessor;
        private readonly IRepository<QueuedSms, Guid> queuedSmsRepository;

        public MessageService(
            IMessageTemplateService messageTemplateService,
            IRepository<QueuedEmail, Guid> queuedEmailRepository,
            IRepository<QueuedSms, Guid> queuedSmsRepository,
            ITokenizer tokenizer,
            IEnumerable<IMessageTokensProvider> tokenProviders,
            IWorkContextAccessor workContextAccessor, IEventBus eventBus)
            : base(queuedEmailRepository, eventBus)
        {
            this.tokenizer = tokenizer;
            this.tokenProviders = tokenProviders;
            this.workContextAccessor = workContextAccessor;
            this.messageTemplateService = messageTemplateService;
            this.queuedSmsRepository = queuedSmsRepository;
        }

        public Guid SendEmailMessage(string messageTemplate, IList<Token> tokens, string toEmailAddress, string toName = null)
        {
            var template = messageTemplateService.GetTemplate(messageTemplate);
            if (template == null || !template.Enabled)
            {
                return Guid.Empty;
            }

            var workContext = workContextAccessor.GetContext();

            foreach (var tokenProvider in tokenProviders)
            {
                tokenProvider.GetTokens(messageTemplate, workContext, tokens);
            }

            return SendMessage(template, tokens, toEmailAddress, toName);
        }

        public Guid SendEmailMessage(string subject, string body, string toEmailAddress, string toName = null)
        {
            var mailMessage = new MailMessage
            {
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(new MailAddress(toEmailAddress, toName));

            return SendEmailMessage(mailMessage);
        }

        public Guid SendEmailMessage(MailMessage mailMessage)
        {
            var mailMessageWrap = new MailMessageWrapper(mailMessage);

            var queuedEmail = new QueuedEmail
            {
                Priority = 5,
                ToAddress = mailMessage.To[0].Address,
                ToName = mailMessage.To[0].DisplayName,
                Subject = mailMessage.Subject,
                MailMessage = mailMessageWrap.ToString(),
                CreatedOnUtc = DateTime.UtcNow
            };

            Insert(queuedEmail);

            return queuedEmail.Id;
        }

        public Guid SendSmsMessage(string messageTemplate, IList<Token> tokens, string fromNumber, string toNumber)
        {
            var template = messageTemplateService.GetTemplate(messageTemplate);
            if (template == null || !template.Enabled)
            {
                return Guid.Empty;
            }

            var workContext = workContextAccessor.GetContext();

            foreach (var tokenProvider in tokenProviders)
            {
                tokenProvider.GetTokens(messageTemplate, workContext, tokens);
            }

            var bodyReplaced = tokenizer.Replace(template.Body, tokens, true);

            return SendSmsMessage(fromNumber, toNumber, bodyReplaced);
        }

        public Guid SendSmsMessage(string fromNumber, string toNumber, string message)
        {
            var queuedSms = new QueuedSms
            {
                Id = Guid.NewGuid(),
                Priority = 5,
                FromNumber = fromNumber,
                ToNumber = toNumber,
                Message = message,
                CreatedOnUtc = DateTime.UtcNow
            };

            queuedSmsRepository.Insert(queuedSms);

            return queuedSms.Id;
        }

        private Guid SendMessage(Domain.MessageTemplate messageTemplate,
            IList<Token> tokens, string toEmailAddress, string toName)
        {
            var subject = messageTemplate.Subject ?? string.Empty;
            var body = messageTemplate.Body ?? string.Empty;

            //Replace subject and body tokens
            var subjectReplaced = tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = tokenizer.Replace(body, tokens, true);

            return SendEmailMessage(subjectReplaced, bodyReplaced, toEmailAddress, toName);
        }

        protected override IOrderedQueryable<QueuedEmail> MakeDefaultOrderBy(IQueryable<QueuedEmail> queryable)
        {
            return queryable.OrderByDescending(x => x.CreatedOnUtc);
        }

        public IList<QueuedEmail> GetQueuedEmails(int maxSendTries, bool loadNotSentItemsOnly, bool loadNewest, int pageSize)
        {
            var query = Repository.Table.Where(x => x.SentTries < maxSendTries);

            if (loadNotSentItemsOnly)
            {
                query = query.Where(x => x.SentOnUtc == null);
            }

            query = query.OrderByDescending(x => x.Priority);

            query = loadNewest ? ((IOrderedQueryable<QueuedEmail>)query).ThenByDescending(x => x.CreatedOnUtc) : ((IOrderedQueryable<QueuedEmail>)query).ThenBy(x => x.CreatedOnUtc);

            return query.Take(pageSize).ToList();
        }

        public IList<QueuedSms> GetQueuedSms(int maxSendTries, bool loadNotSentItemsOnly, bool loadNewest, int pageSize)
        {
            var query = queuedSmsRepository.Table.Where(x => x.SentTries < maxSendTries);

            if (loadNotSentItemsOnly)
            {
                query = query.Where(x => x.SentOnUtc == null);
            }

            query = query.OrderByDescending(x => x.Priority);

            query = loadNewest ? ((IOrderedQueryable<QueuedSms>)query).ThenByDescending(x => x.CreatedOnUtc) : ((IOrderedQueryable<QueuedSms>)query).ThenBy(x => x.CreatedOnUtc);

            return query.Take(pageSize).ToList();
        }

        public void Update(QueuedSms smsMessage)
        {
            queuedSmsRepository.Update(smsMessage);
        }
    }
}