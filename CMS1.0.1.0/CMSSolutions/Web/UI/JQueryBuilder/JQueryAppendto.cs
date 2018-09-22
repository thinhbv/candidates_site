namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryAppendTo : JQuery
    {
        private readonly object target;

        public JQueryAppendTo(object target)
        {
            this.target = target;
        }

        public override string Build()
        {
            if (target is string)
            {
                return string.Format(".appendTo(\"{0}\")", JQueryUtility.EncodeJsString(target.ToString()));
            }

            return string.Format(".appendTo({0})", target);
        }
    }
}