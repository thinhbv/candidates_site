using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    internal class ControlFormRazorView : IView
    {
        private readonly IList<ResourceType> additionalResources;
        private readonly IList<string> additionalScripts;
        private readonly IDictionary<string, bool> additionalStyles;

        public ControlFormRazorView(ControllerContext controllerContext, IView view, IList<ResourceType> additionalResources, IList<string> additionalScripts, IDictionary<string, bool> additionalStyles)
        {
            this.additionalResources = additionalResources;
            this.additionalScripts = additionalScripts;
            this.additionalStyles = additionalStyles;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            //var page = instance as Mvc.WebViewPage<dynamic>;
            //if (page == null)
            //{
            //    throw new InvalidOperationException();
            //}

            //page.ViewContext = viewContext;
            //page.InitHelpers();

            //foreach (var resource in additionalResources)
            //{
            //    ResourcesManager.LookupResources(resource, page.Script, page.Style);
            //}

            //foreach (var script in additionalScripts)
            //{
            //    page.Script.Include(script);
            //}

            //foreach (var style in additionalStyles)
            //{
            //    page.Style.Include(style.Key, style.Value);
            //}
            throw new NotImplementedException();
        }
    }
}