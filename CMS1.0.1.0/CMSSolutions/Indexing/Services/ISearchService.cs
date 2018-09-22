using System;
using System.Globalization;
using System.Linq;
using CMSSolutions.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;

namespace CMSSolutions.Indexing.Services
{
    public interface ISearchService : IDependency
    {
        PagedList<T> Query<T>(string query, int skip, int? take, string[] searchFields, string culture, Func<ISearchHit, T> shapeResult);
    }

    [Feature(Constants.Areas.Indexing)]
    public class SearchService : ISearchService
    {
        private readonly IIndexManager indexManager;

        public SearchService(IIndexManager indexManager)
        {
            this.indexManager = indexManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private ISearchBuilder Search()
        {
            return indexManager.HasIndexProvider()
                ? indexManager.GetSearchIndexProvider().CreateSearchBuilder("Search")
                : new NullSearchBuilder();
        }

        PagedList<T> ISearchService.Query<T>(string query, int page, int? pageSize, string[] searchFields, string culture, Func<ISearchHit, T> shapeResult)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new PagedList<T>(Enumerable.Empty<T>());

            var searchBuilder = Search().Parse(searchFields, query);

            if (!string.IsNullOrEmpty(culture))
            {
                var cultureInfo = new CultureInfo(culture);
                searchBuilder.WithField("culture", cultureInfo.LCID).AsFilter();
            }

            var totalCount = searchBuilder.Count();
            if (pageSize != null)
                searchBuilder = searchBuilder
                    .Slice((page > 0 ? page - 1 : 0) * (int)pageSize, (int)pageSize);

            var searchResults = searchBuilder.Search();

            var pageOfItems = new PagedList<T>(searchResults.Select(shapeResult), page, pageSize != null ? (int)pageSize : totalCount, totalCount);

            return pageOfItems;
        }
    }
}