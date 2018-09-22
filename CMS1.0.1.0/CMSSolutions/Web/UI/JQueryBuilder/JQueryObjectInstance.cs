namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryObjectInstance : JQuery
    {
        private readonly string name;

        public JQueryObjectInstance(string name)
        {
            this.name = name;
        }

        public JQueryObjectInstance(JQueryDeclare declare)
        {
            name = declare.Name;
        }

        protected override bool ReturnJQuery
        {
            get { return true; }
        }

        public override string Build()
        {
            return name;
        }
    }
}