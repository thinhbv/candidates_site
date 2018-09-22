namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryValidate : JQuery
    {
        private readonly JQuery[] submitHandlers;

        public JQueryValidate(params JQuery[] submitHandlers)
        {
            this.submitHandlers = submitHandlers;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            return string.Format(".validate({{ submitHandler: function(form){{ {0} }} }})", new JQueryHandlers(submitHandlers));
        }
    }
}