using System;

namespace CMSSolutions.Indexing.Services
{
    public interface IIndexStatisticsProvider : IDependency
    {
        DateTime GetLastIndexedUtc(string indexName);

        IndexingStatus GetIndexingStatus(string indexName);
    }
}