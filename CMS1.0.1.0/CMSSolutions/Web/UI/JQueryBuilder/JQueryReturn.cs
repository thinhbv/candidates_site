using System.Globalization;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryReturn : JQuery
    {
        private bool? returnValue;

        public JQueryReturn(bool? returnValue)
        {
            this.returnValue = returnValue;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            if (returnValue.HasValue)
            {
                return "return " + returnValue.Value.ToString(CultureInfo.InvariantCulture).ToLowerInvariant() + ";";
            }

            return "return;";
        }
    }
}