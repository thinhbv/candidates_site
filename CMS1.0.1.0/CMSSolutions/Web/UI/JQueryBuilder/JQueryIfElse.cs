namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryIfElse : JQuery
    {
        private readonly JQuery condition;
        private readonly JQuery trueStatement;
        private readonly JQuery elseStatement;

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public JQueryIfElse(JQuery condition, JQuery trueStatement)
        {
            this.condition = condition;
            this.trueStatement = trueStatement;
        }

        public JQueryIfElse(JQuery condition, JQuery trueStatement, JQuery elseStatement)
        {
            this.condition = condition;
            this.trueStatement = trueStatement;
            this.elseStatement = elseStatement;
        }

        public override string Build()
        {
            if (elseStatement == null)
            {
                return string.Format("if({0}) {{ {1} }}", condition, trueStatement);
            }

            return string.Format("if({0}) {{ {1} }} else {{ {2} }}", condition, trueStatement, elseStatement);
        }
    }
}