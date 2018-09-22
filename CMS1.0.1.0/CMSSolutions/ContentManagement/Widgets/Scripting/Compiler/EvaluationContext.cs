using System;
using System.Collections.Generic;
using CMSSolutions.ContentManagement.Widgets.Scripting.Ast;

namespace CMSSolutions.ContentManagement.Widgets.Scripting.Compiler
{
    public class EvaluationContext
    {
        public AbstractSyntaxTree Tree { get; set; }

        public Func<string, IList<object>, object> MethodInvocationCallback { get; set; }
    }
}