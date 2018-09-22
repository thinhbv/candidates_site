namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryPrependTo : JQuery
    {
        private readonly object target;

        public JQueryPrependTo(object target)
        {
            this.target = target;
        }

        public override string Build()
        {
            if (target is string)
            {
                return string.Format(".prependTo(\"{0}\")", JQueryUtility.EncodeJsString(target.ToString()));
            }

            return string.Format(".prependTo({0})", target);
        }
    }
}