using System.Collections.Generic;
using System.Linq;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryNativeObject : JQuery
    {
        private readonly IList<JQueryAssignment> assignments;
        private readonly string objectName;

        public JQueryNativeObject(string objectName)
        {
            this.objectName = objectName;
        }

        public JQueryNativeObject(params JQueryAssignment[] assignments)
        {
            this.assignments = new List<JQueryAssignment>();
            if (assignments == null || assignments.Length <= 0) return;
            foreach (var assignment in assignments)
            {
                this.assignments.Add(new JQueryAssignment(assignment, true));
            }
        }

        public override string Build()
        {
            if (string.IsNullOrEmpty(objectName))
            {
                if (assignments.Count == 0)
                {
                    return "{}";
                }

                return "{" + string.Join(", ", assignments.Select(x => x.Build())) + "}";
            }

            return objectName;
        }
    }
}