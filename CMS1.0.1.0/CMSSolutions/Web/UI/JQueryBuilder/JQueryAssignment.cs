using System;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryAssignment : JQuery
    {
        private readonly object left;
        private readonly object right;
        private readonly bool insideObject;

        public JQueryAssignment(object left, object right, bool insideObject = false)
        {
            this.left = left;
            this.right = right;
            this.insideObject = insideObject;
        }

        public JQueryAssignment(JQueryAssignment assignment, bool insideObject = false)
        {
            left = assignment.left;
            right = assignment.right;
            this.insideObject = insideObject;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            if (insideObject)
            {
                if (right == null)
                {
                    return string.Format("\"{0}\": null", left);
                }

                var str = right as string;
                if (str != null)
                {
                    return string.Format("\"{0}\": \"{1}\"", left, JQueryUtility.EncodeJsString(str));
                }

                if (right is DateTime)
                {
                    var dt = (DateTime)right;
                    return string.Format("\"{0}\": new Date({1})", left, JQueryUtility.GetUnixMilliseconds(dt));
                }

                return string.Format("\"{0}\": {1}", left, right);
            }
            else
            {
                if (right == null)
                {
                    return string.Format("{0} = null;", left);
                }

                var str = right as string;
                if (str != null)
                {
                    return string.Format("{0} = \"{1}\";", left, JQueryUtility.EncodeJsString(str));
                }

                if (right is DateTime)
                {
                    var dt = (DateTime)right;
                    return string.Format("{0} = new Date({1});", left, JQueryUtility.GetUnixMilliseconds(dt));
                }

                return string.Format("{0} = {1};", left, right);
            }
        }
    }
}