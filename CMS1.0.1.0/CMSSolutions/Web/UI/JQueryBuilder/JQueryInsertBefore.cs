namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryInsertBefore : JQuery
    {
        private readonly object target;

        public JQueryInsertBefore(object target)
        {
            this.target = target;
        }

        public override string Build()
        {
            if (target is string)
            {
                return string.Format(".insertBefore(\"{0}\")", JQueryUtility.EncodeJsString(target.ToString()));
            }

            return string.Format(".insertBefore({0})", target);
        }
    }
}