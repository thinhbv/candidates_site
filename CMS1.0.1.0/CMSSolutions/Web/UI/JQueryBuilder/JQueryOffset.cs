namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryOffset : JQuery
    {
        private readonly object coordinates;

        public JQueryOffset()
        {
        }

        public JQueryOffset(object coordinates)
        {
            this.coordinates = coordinates;
        }

        protected override bool ReturnJQuery
        {
            get
            {
                if (coordinates == null)
                {
                    return false;
                }
                return base.ReturnJQuery;
            }
        }

        public override string Build()
        {
            if (coordinates == null)
            {
                return ".offset()";
            }

            if (coordinates is string)
            {
                return string.Format(".offset(\"{0}\")", JQueryUtility.EncodeJsString(coordinates.ToString()));
            }

            return string.Format(".offset({0})", coordinates);
        }
    }
}