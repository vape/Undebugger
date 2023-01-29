using Undebugger.Scripts.Services.UI;
using UnityEngine;

namespace Undebugger.UI
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
