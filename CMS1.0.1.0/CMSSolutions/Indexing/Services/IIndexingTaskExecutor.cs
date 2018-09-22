namespace CMSSolutions.Indexing.Services
{
    public interface IIndexingTaskExecutor : IDependency
    {
        bool DeleteIndex(string indexName);

        bool UpdateIndex(string indexName);
    }
}