using Undebugger.Services.UI;
using UnityEngine;

namespace Undebugger.UI.Controls
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    public class CloseMenuButton : MonoBehaviour
    {
        public void TryClose()
        {
            if (UIService.Instance != null)
            {
                UIService.Instance.CloseMenu();
            }
        }
    }
}
