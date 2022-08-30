using System.Collections.Generic;

namespace Deszz.Undebugger.UI.Windows
{
    public interface IWindowButtonsProvider
    {
        IEnumerable<WindowButtonPreset> GetWindowButtonPresets();
    }
}
