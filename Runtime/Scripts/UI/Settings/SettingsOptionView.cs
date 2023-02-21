using UnityEngine;

namespace Undebugger.UI.Settings
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal abstract class SettingsOptionView : MonoBehaviour
    {
        public virtual void Refresh()
        { }
    }
}
