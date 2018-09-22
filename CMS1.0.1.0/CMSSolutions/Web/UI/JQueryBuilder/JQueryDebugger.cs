﻿namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryDebugger : JQuery
    {
        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            return "debugger;";
        }
    }
}