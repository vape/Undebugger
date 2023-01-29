using UnityEngine;

namespace Undebugger.UI
{
    public class CloseMenuButton : MonoBehaviour
    {
        public void TryClose()
        {
            if (UndebuggerManager.Instance != null)
            {
                UndebuggerManager.Instance.TryCloseMenu();
            }
        }
    }
}
