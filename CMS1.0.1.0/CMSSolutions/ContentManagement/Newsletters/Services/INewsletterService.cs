using System;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.Collections;
using CMSSolutions.Collections.Generic;
using CMSSolutions.ContentManagement.Newsletters.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;
using CMSSolutions.Web.Security;
using CMSSolutions.Web.Security.Services;

namespace CMSSolutions.ContentManagement.Newsletters.Services
{
    public interface INewsletterService : IGenericService<Newsletter, Guid>, IDependency
    {
        Newsletter GetByLanguage(Guid id, string cultureCode);

        /// <summary>
        /// <para>First: User Name</para>
        /// <para>Second: E-mail Address</para>
        /// <para>Third: Preferred Language (Culture Code)</para>
        /// </summary>
        /// <returns></returns>
        TripleList<string, string, string> GetSubscribers();
    }

    [Feature(Constants.Areas.Newsletters)]
    public class NewsletterService : GenericService<Newsletter, Guid>, INewsletterService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;
        private readonly IMembershipService membershipService;

        public NewsletterService(
            IRepository<Newsletter, Guid> repository,
            IEventBus eventBus,
            ICacheManager cacheManager,
            ISignals signals,
            IMembershipService membershipService)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
            this.membershipService = membershipService;
        }

        protected override IOrderedQueryable<Newsletter> MakeDefaultOrderBy(IQueryable<Newsletter> queryable)
        {
            return queryable.OrderByDescending(x => x.DateCreated);
        }

        public override void Insert(Newsletter record)
        {
            base.Insert(record);
            signals.Trigger("Newsletters_Changed");
        }

        public override void Update(Newsletter record)
        {
            base.Update(record);
            signals.Trigger("Newsletters_Changed");
        }

        public override void Delete(Newsletter record)
        {
            base.Delete(record);
            signals.Trigger("Newsletters_Changed");
        }

        #region INewsletterService Members

        public Newsletter GetByLanguage(Guid id, string cultureCode)
        {
            return Repository.Table.FirstOrDefault(x => x.RefId == id && x.CultureCode == cultureCode);
        }

        public TripleList<string, string, string> GetSubscribers()
        {
            var userReceiveNewsletters = membershipService.GetProfileEntriesByKey(NewsletterUserProfileProvider.ReceiveNewsletters);
            var userLanguages = membershipService.GetProfileEntriesByKey(SecurityUserProfileProvider.PreferredLanguage);

            var result = new TripleList<string, string, string>();

            if (!userReceiveNewsletters.IsNullOrEmpty())
            {
                var userIds = userReceiveNewsletters.Where(x => bool.Parse(x.Value)).Select(x => x.UserId);
                var users = membershipService.GetUsers(userIds);

                foreach (var user in users)
                {
                    var preferredLanguage = userLanguages.FirstOrDefault(x => x.UserId == user.Id);

                    result.Add(new Triple<string, string, string>(
                        user.ToString(),
                        user.Email,
                        preferredLanguage == null
                            ? string.Empty
                            : preferredLanguage.Value));
                }
            }

            return result;
        }

        #endregion INewsletterService Members
    }
}