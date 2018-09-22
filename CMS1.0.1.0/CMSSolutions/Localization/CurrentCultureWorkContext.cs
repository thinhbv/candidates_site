using System;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Localization
{
    [Feature(Constants.Areas.Localization)]
    public class CurrentCultureWorkContext : IWorkContextStateProvider
    {
        private readonly ICultureManager cultureManager;

        public CurrentCultureWorkContext(ICultureManager cultureManager)
        {
            this.cultureManager = cultureManager;
        }

        #region IWorkContextStateProvider Members

        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name == "CurrentCulture")
            {
                return ctx => (T)(object)cultureManager.GetCurrentCulture(ctx.HttpContext);
            }
            return null;
        }

        #endregion IWorkContextStateProvider Members
    }
}