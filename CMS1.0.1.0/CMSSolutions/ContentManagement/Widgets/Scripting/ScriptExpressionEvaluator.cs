using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.ContentManagement.Widgets.Scripting.Ast;
using CMSSolutions.ContentManagement.Widgets.Scripting.Compiler;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;

namespace CMSSolutions.ContentManagement.Widgets.Scripting
{
    [Feature(Constants.Areas.Widgets)]
    public class ScriptExpressionEvaluator : IScriptExpressionEvaluator
    {
        private readonly ICacheManager cacheManager;

        public ScriptExpressionEvaluator(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public object Evaluate(string expression, IEnumerable<IGlobalMethodProvider> providers)
        {
            var expr = cacheManager.Get(expression, ctx =>
            {
                var ast = ParseExpression(expression);
                return new { Tree = ast, Errors = ast.GetErrors().ToList() };
            });

            if (expr.Errors.Any())
            {
                throw new CMSException(T("Syntax error: {0}", expr.Errors.First().Message));
            }

            var result = EvaluateExpression(expr.Tree, providers);
            if (result.IsError)
            {
                throw new CMSException(result.ErrorValue.Message);
            }

            return result.Value;
        }

        private AbstractSyntaxTree ParseExpression(string expression)
        {
            return new Parser(expression).Parse();
        }

        private EvaluationResult EvaluateExpression(AbstractSyntaxTree tree, IEnumerable<IGlobalMethodProvider> providers)
        {
            var context = new EvaluationContext
            {
                Tree = tree,
                MethodInvocationCallback = (m, args) => Evaluate(providers, m, args)
            };
            return new Interpreter().Evalutate(context);
        }

        private object Evaluate(IEnumerable<IGlobalMethodProvider> globalMethodProviders, string name, IEnumerable<object> args)
        {
            var globalMethodContext = new GlobalMethodContext
            {
                FunctionName = name,
                Arguments = args.ToArray(),
                Result = null
            };

            foreach (var globalMethodProvider in globalMethodProviders)
            {
                globalMethodProvider.Process(globalMethodContext);
            }

            return globalMethodContext.Result;
        }
    }
}