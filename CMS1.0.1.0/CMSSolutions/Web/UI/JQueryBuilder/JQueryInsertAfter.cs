namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryInsertAfter : JQuery
    {
        private readonly object target;

        public JQueryInsertAfter(object target)
        {
            this.target = target;
        }

        public override string Build()
        {
            if (target is string)
            {
                return string.Format(".insertAfter(\"{0}\")", JQueryUtility.EncodeJsString(target.ToString()));
            }

            return string.Format(".insertAfter({0})", target);
        }
    }
}