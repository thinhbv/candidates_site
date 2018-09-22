using CMSSolutions.Events;

namespace CMSSolutions.Tasks
{
    public interface IBackgroundTask : IEventHandler
    {
        void Sweep();
    }
}