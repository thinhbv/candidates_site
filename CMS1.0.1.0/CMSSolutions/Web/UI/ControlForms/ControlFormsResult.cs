using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlFormsResult : BaseControlFormResult
    {
        private readonly IList<BaseControlFormResult> forms;

        public ControlFormsResult()
        {
            forms = new List<BaseControlFormResult>();
        }

        public ControlFormsResult(params BaseControlFormResult[] forms)
        {
            this.forms = new List<BaseControlFormResult>(forms);
        }

        public void AddForm(BaseControlFormResult form)
        {
            forms.Add(form);
        }

        public override IEnumerable<ResourceType> GetAdditionalResources(ControllerContext context)
        {
            return forms.Where(x => x != null).SelectMany(form => form.GetAdditionalResources(context));
        }

        public override bool OverrideExecuteResult(ControllerContext context)
        {
            if (forms.Any(form => form != null && form.OverrideExecuteResult(context)))
            {
                return true;
            }

            return base.OverrideExecuteResult(context);
        }

        public override string GenerateControlFormUI(ControllerContext controllerContext)
        {
            return string.Join("", forms.Where(x => x != null).Select(f => f.GenerateControlFormUI(controllerContext)));
        }
    }
}