using System.Collections.Generic;

namespace CMSSolutions.ContentManagement.Widgets.Scripting.Ast
{
    public class AbstractSyntaxTree
    {
        public AstNode Root { get; set; }

        public IEnumerable<ErrorAstNode> GetErrors()
        {
            return new ErrorNodeCollector().Collect(Root);
        }
    }
}