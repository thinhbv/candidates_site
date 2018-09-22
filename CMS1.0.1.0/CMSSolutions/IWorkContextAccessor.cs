using System.Web;

namespace CMSSolutions
{
    public interface IWorkContextAccessor
    {
        WorkContext GetContext();

        WorkContext GetContext(HttpContextBase httpContext);

        IWorkContextScope CreateWorkContextScope();

        IWorkContextScope CreateWorkContextScope(HttpContextBase httpContext);
    }
}