using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlContentResult : BaseControlFormResult
    {
        private readonly string content;

        public ControlContentResult(string content)
        {
            this.content = content;
        }

        public Func<IEnumerable<ResourceType>> AdditionResources { get; set; }

        #region Overrides of BaseControlFormResult

        public override string GenerateControlFormUI(ControllerContext controllerContext)
        {
            return content;
        }

        public override IEnumerable<ResourceType> GetAdditionalResources(ControllerContext context)
        {
            return AdditionResources != null ? AdditionResources() : base.GetAdditionalResources(context);
        }

        #endregion Overrides of BaseControlFormResult
    }
}