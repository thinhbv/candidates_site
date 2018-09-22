using System;
using System.Globalization;
using System.Web;
using Autofac.Core;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Web.Security;
using CMSSolutions.Web.UI.Navigation;
using CMSSolutions.Web.UI.Notify;

namespace CMSSolutions
{
    public abstract class WorkContext : IUnitOfWorkDependency
    {
        public abstract T Resolve<T>();

        public abstract T ResolveOptional<T>() where T : class;

        public abstract T ResolveWithParameters<T>(params Parameter[] parameters);

        public abstract object Resolve(Type serviceType);

        public abstract T ResolveNamed<T>(string name);

        public abstract bool TryResolve<T>(out T service);

        public abstract T GetState<T>(string name);

        public abstract void SetState<T>(string name, T value);

        public HttpContextBase HttpContext
        {
            get { return GetState<HttpContextBase>("HttpContext"); }
        }

        public string DomainName
        {
            get
            {
                return this.HttpContext.Request.Url != null ? HttpContext.Request.Url.Host : "localhost";
            }
        }

        public dynamic Layout
        {
            get { return GetState<object>("Layout"); }
        }

        public abstract Breadcrumbs Breadcrumbs { get; }

        public ExtensionDescriptor CurrentTheme
        {
            get { return GetState<ExtensionDescriptor>("CurrentTheme"); }
            set { SetState("CurrentTheme", value); }
        }

        public string CurrentMobileTheme
        {
            get { return GetState<string>("CurrentMobileTheme"); }
        }

        public string CurrentCulture
        {
            get { return GetState<string>("CurrentCulture"); }
        }

        public DateTimeFormatInfo DateTimeFormat
        {
            get { return GetState<DateTimeFormatInfo>("DateTimeFormat"); }
        }

        public TimeZoneInfo CurrentTimeZone
        {
            get { return GetState<TimeZoneInfo>("CurrentTimeZone"); }
        }

        public IUserInfo CurrentUser
        {
            get { return GetState<IUserInfo>("CurrentUser"); }
        }

        public INotifier Notifier { get { return Resolve<INotifier>(); } }

        public int DefaultPageSize
        {
            get
            {
                var value = GetState<int>("DefaultPageSize");
                return value <= 0 ? 15 : value;
            }
        }
    }
}