namespace CMSSolutions.ContentManagement.Widgets.RuleEngine
{
    public interface IRuleProvider : IDependency
    {
        void Process(RuleContext ruleContext);
    }
}