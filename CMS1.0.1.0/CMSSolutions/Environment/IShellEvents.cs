using CMSSolutions.Events;

namespace CMSSolutions.Environment
{
    public interface IShellEvents : IEventHandler
    {
        void Activated();

        void Terminating();
    }
}