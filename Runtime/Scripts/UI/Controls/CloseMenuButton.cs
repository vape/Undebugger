using Undebugger.Services.UI;
using UnityEngine;

namespace Undebugger.UI.Controls
{
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
