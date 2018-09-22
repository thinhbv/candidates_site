namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryClone : JQuery
    {
        private readonly bool withDataAndEvents;
        private readonly bool deepWithDataAndEvents;

        public JQueryClone(bool withDataAndEvents = false, bool deepWithDataAndEvents = false)
        {
            this.withDataAndEvents = withDataAndEvents;
            this.deepWithDataAndEvents = deepWithDataAndEvents;
        }

        protected override bool ReturnJQuery
        {
            get
            {
                if (withDataAndEvents == false && deepWithDataAndEvents == false)
                {
                    return false;
                }
                return base.ReturnJQuery;
            }
        }

        public override string Build()
        {
            if (withDataAndEvents == false && deepWithDataAndEvents == false)
            {
                return ".clone()";
            }

            return string.Format(".clone({0}, {1})", withDataAndEvents, deepWithDataAndEvents);
        }
    }
}