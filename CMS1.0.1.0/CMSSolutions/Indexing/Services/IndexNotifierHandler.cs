using System;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Indexing.Services
{
    [Feature(Constants.Areas.Indexing)]
    public class IndexNotifierHandler : IIndexNotifierHandler
    {
        private readonly Lazy<IIndexingTaskExecutor> _indexingTaskExecutor;

        public IndexNotifierHandler(Lazy<IIndexingTaskExecutor> indexingTaskExecutor)
        {
            _indexingTaskExecutor = indexingTaskExecutor;
        }

        public void UpdateIndex(string indexName)
        {
            _indexingTaskExecutor.Value.UpdateIndex(indexName);
        }

        public int Priority { get { return 0; } }
    }
}