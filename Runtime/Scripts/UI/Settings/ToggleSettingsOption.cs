using System;

namespace Undebugger.UI.Settings
{
    internal class ToggleSettingsOption : SettingsOption
    {
        public Func<bool> GetValue
        { get; set; }
        public Action<bool> SetValue
        { get; set; }
    }
}
