using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Indexing
{
    public interface IIndexManager : IDependency
    {
        bool HasIndexProvider();

        IIndexProvider GetSearchIndexProvider();
    }

    [Feature(Constants.Areas.Indexing)]
    public class DefaultIndexManager : IIndexManager
    {
        private readonly IEnumerable<IIndexProvider> indexProviders;

        public DefaultIndexManager(IEnumerable<IIndexProvider> indexProviders)
        {
            this.indexProviders = indexProviders;
        }

        #region IIndexManager Members

        public bool HasIndexProvider()
        {
            return indexProviders.AsQueryable().Any();
        }

        public IIndexProvider GetSearchIndexProvider()
        {
            return indexProviders.AsQueryable().FirstOrDefault();
        }

        #endregion IIndexManager Members
    }
}