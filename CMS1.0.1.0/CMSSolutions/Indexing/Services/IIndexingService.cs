namespace CMSSolutions.Indexing.Services
{
    public interface IIndexingService : IDependency
    {
        void RebuildIndex(string indexName);

        void UpdateIndex(string indexName);

        IndexEntry GetIndexEntry(string indexName);
    }
}