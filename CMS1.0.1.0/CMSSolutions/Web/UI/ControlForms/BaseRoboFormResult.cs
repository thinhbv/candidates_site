using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.Web.Fakes;
using CMSSolutions.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public abstract class BaseControlFormResult : ViewResult
    {
        private readonly IDictionary<string, string> hiddenValues;

        protected BaseControlFormResult()
        {
            hiddenValues = new Dictionary<string, string>();
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public ControlFormProvider FormProvider { get; set; }

        protected IDictionary<string, string> HiddenValues { get { return hiddenValues; } }

        public void AddHiddenValue(string name, string value)
        {
            hiddenValues.Add(name, value);
        }

        public virtual IEnumerable<ResourceType> GetAdditionalResources(ControllerContext context)
        {
            return Enumerable.Empty<ResourceType>();
        }

        protected override ViewEngineResult FindView(ControllerContext context)
        {
            var result = ViewEngineCollection.FindView(context, "ControlFormResult_", null);

            var controller = context.Controller as BaseController;
            if (controller != null)
            {
                var additionalResources = GetAdditionalResources(context).ToList();
                if (additionalResources.Any())
                {
                    var workContext = controller.WorkContext;
                    var scriptRegister = new ScriptRegister(workContext);
                    var styleRegister = new StyleRegister(workContext);

                    foreach (var resource in additionalResources)
                    {
                        ResourcesManager.LookupResources(resource, scriptRegister, styleRegister);
                    }
                }
            }

            return result;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (OverrideExecuteResult(context))
            {
                return;
            }

            if (!string.IsNullOrEmpty(Title))
            {
                context.Controller.ViewBag.Title = Title;
            }

            // Generate Control Form content
            var controlFormContent = GenerateControlFormUI(context);

            var faceHttpContext = new FakeHttpContext(context.HttpContext);
            var fakeContext = new ControllerContext(faceHttpContext, context.RouteData, context.Controller);
            ViewData = context.Controller.ViewData;
            base.ExecuteResult(fakeContext);

            using (var reader = new StreamReader(faceHttpContext.Response.OutputStream))
            {
                var str = reader.ReadToEnd();

                str = str.Replace("[THIS_IS_CONTENT_HOLDER_FOR_ROBO_FORM]", controlFormContent);

                context.HttpContext.Response.Write(str);
            }
        }

        public virtual bool OverrideExecuteResult(ControllerContext context)
        {
            return false;
        }

        public abstract string GenerateControlFormUI(ControllerContext controllerContext);
    }
}