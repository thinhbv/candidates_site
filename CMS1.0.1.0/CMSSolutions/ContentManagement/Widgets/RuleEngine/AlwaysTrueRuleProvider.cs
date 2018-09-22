using System;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Widgets.RuleEngine
{
    [Feature(Constants.Areas.Widgets)]
    public class AlwaysTrueRuleProvider : IRuleProvider
    {
        public void Process(RuleContext ruleContext)
        {
            if (ruleContext.FunctionName == "add")
            {
                ruleContext.Result = Convert.ToInt32(ruleContext.Arguments[0]) + Convert.ToInt32(ruleContext.Arguments[1]);
                return;
            }

            ruleContext.Result = true;
        }
    }
}