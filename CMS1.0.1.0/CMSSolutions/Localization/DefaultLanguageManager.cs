using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Localization
{
    [Feature(Constants.Areas.Localization)]
    public class LanguageManager : ILanguageManager
    {
        private readonly IWorkContextAccessor workContextAccessor;

        public LanguageManager(IWorkContextAccessor workContextAccessor)
        {
            this.workContextAccessor = workContextAccessor;
        }

        public IList<Language> GetActiveLanguages(string themeLanguages, bool all)
        {
            var workContext = workContextAccessor.GetContext();
            var repository = workContext.Resolve<IRepository<Domain.Language, int>>();
            if (all)
            {
                return repository.Table
                    .Where(x => x.Active && !x.IsBlocked)
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new Language
                    {
                        CultureCode = x.CultureCode,
                        Name = x.Name
                    }).ToList();
            }

            return repository.Table
                .Where(x => x.Active && x.Theme == themeLanguages && !x.IsBlocked)
                .OrderBy(x => x.SortOrder)
                .Select(x => new Language
                {
                    CultureCode = x.CultureCode,
                    Name = x.Name
                }).ToList();
        }
    }
}