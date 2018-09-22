using CMSSolutions.Events;

namespace CMSSolutions.Indexing
{
    public interface IIndexNotifierHandler : IEventHandler
    {
        void UpdateIndex(string indexName);
    }
}