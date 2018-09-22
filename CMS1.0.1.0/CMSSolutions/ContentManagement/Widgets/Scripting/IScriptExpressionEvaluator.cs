using System.Collections.Generic;

namespace CMSSolutions.ContentManagement.Widgets.Scripting
{
    public interface IScriptExpressionEvaluator : ISingletonDependency
    {
        object Evaluate(string expression, IEnumerable<IGlobalMethodProvider> providers);
    }
}