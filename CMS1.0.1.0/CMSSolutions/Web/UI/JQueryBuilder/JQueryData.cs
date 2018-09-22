namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryData : JQuery
    {
        private readonly string name;
        private readonly object value;

        public JQueryData(string name, object value = null)
        {
            this.name = name;
            this.value = value;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            if (value == null)
            {
                return string.Format(".data('{0}')", name);
            }

            if (value is string)
            {
                return string.Format(".data('{0}', \"{1}\")", name, JQueryUtility.EncodeJsString(value.ToString()));
            }

            return string.Format(".data('{0}', {1})", name, value);
        }
    }
}