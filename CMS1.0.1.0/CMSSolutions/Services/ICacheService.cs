using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Services
{
    public interface ICacheService<TRecord, in TKey>
    {
        TRecord GetByIdCache(TKey id);

        IList<TRecord> GetAllCache();

        IList<TRecord> GetRecords(
            ControlGridFormRequest request, 
            out int totalRecords,
            Expression<Func<TRecord, bool>> predicate = null,
            params Expression<Func<TRecord, dynamic>>[] includePaths);

        IList<TRecord> ResetCache();
    }
}
