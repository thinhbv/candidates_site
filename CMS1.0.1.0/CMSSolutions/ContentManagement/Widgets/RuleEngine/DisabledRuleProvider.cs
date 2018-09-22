using System;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Widgets.RuleEngine
{
    [Feature(Constants.Areas.Widgets)]
    public class DisabledRuleProvider : IRuleProvider
    {
        public void Process(RuleContext ruleContext)
        {
            if (!string.Equals(ruleContext.FunctionName, "disabled", StringComparison.OrdinalIgnoreCase))
                return;
            ruleContext.Result = false;
        }
    }
}