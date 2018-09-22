namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryReplaceAll : JQuery
    {
        private readonly object target;

        public JQueryReplaceAll(object target)
        {
            this.target = target;
        }

        public override string Build()
        {
            if (target is string)
            {
                return string.Format(".replaceAll(\"{0}\")", JQueryUtility.EncodeJsString(target.ToString()));
            }

            return string.Format(".replaceAll({0})", target);
        }
    }
}