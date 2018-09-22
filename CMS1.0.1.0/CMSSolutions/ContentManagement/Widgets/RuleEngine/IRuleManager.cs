using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.ContentManagement.Widgets.Scripting;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;

namespace CMSSolutions.ContentManagement.Widgets.RuleEngine
{
    public interface IRuleManager : IDependency
    {
        bool Matches(string expression);
    }

    [Feature(Constants.Areas.Widgets)]
    public class RuleManager : IRuleManager
    {
        private readonly IEnumerable<IRuleProvider> ruleProviders;
        private readonly IEnumerable<IScriptExpressionEvaluator> evaluators;

        public RuleManager(IEnumerable<IRuleProvider> ruleProviders, IEnumerable<IScriptExpressionEvaluator> evaluators)
        {
            this.ruleProviders = ruleProviders;
            this.evaluators = evaluators;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public bool Matches(string expression)
        {
            var evaluator = evaluators.FirstOrDefault();
            if (evaluator == null)
            {
                throw new CMSException(T("There are currently not scripting engine enabled"));
            }

            object result;

            try
            {
                result = evaluator.Evaluate(expression, new List<IGlobalMethodProvider> { new GlobalMethodProvider(this) });
            }
            catch (Exception)
            {
                return false;
            }

            if (result == null)
            {
                throw new CMSException(T("Expression is not a boolean value"));
            }
            return (bool)result;
        }

        private class GlobalMethodProvider : IGlobalMethodProvider
        {
            private readonly RuleManager ruleManager;

            public GlobalMethodProvider(RuleManager ruleManager)
            {
                this.ruleManager = ruleManager;
            }

            public void Process(GlobalMethodContext context)
            {
                var ruleContext = new RuleContext
                {
                    FunctionName = context.FunctionName,
                    Arguments = context.Arguments.ToArray(),
                    Result = context.Result
                };

                foreach (var ruleProvider in ruleManager.ruleProviders)
                {
                    ruleProvider.Process(ruleContext);
                }

                context.Result = ruleContext.Result;
            }
        }
    }
}