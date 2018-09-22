using System;
using System.Collections.Generic;

namespace CMSSolutions.Indexing.Services
{
    public interface IIndexingContentProvider : IDependency
    {
        IEnumerable<IDocumentIndex> GetDocuments(Func<string, IDocumentIndex> factory);
    }
}