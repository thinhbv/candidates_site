using CMSSolutions.Events;

namespace CMSSolutions.Environment.Configuration
{
    public interface IShellSettingsManagerEventHandler : IEventHandler
    {
        void Saved(ShellSettings settings);
    }
}