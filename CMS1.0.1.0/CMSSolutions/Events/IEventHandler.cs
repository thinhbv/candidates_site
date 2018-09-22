namespace CMSSolutions.Events
{
    public interface IEventHandler : IDependency
    {
        int Priority { get; }
    }
}