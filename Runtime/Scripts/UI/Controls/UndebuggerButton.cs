using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Undebugger.UI.Controls
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class UndebuggerButton : UndebuggerClickable
    {
        [SerializeField]
        private UnityEvent onClick;

        protected override void OnClick()
        {
            onClick?.Invoke();
        }
    }
}
