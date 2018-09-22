using System;

namespace CMSSolutions
{
    public interface IWorkContextStateProvider : IDependency
    {
        Func<WorkContext, T> Get<T>(string name);
    }
}