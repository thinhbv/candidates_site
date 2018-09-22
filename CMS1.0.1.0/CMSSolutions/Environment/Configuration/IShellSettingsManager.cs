using System.Collections.Generic;

namespace CMSSolutions.Environment.Configuration
{
    public interface IShellSettingsManager
    {
        IEnumerable<ShellSettings> LoadSettings();

        void SaveSettings(ShellSettings settings);
    }
}