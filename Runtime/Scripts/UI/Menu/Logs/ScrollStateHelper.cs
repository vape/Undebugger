using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Logs
{
    internal class ScrollStateHelper : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public bool IsDragging
        { get; private set; }

        [SerializeField]
        private ScrollRect scrollRect;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || !scrollRect.IsActive())
            {
                return;
            }

            IsDragging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
        }
    }
}
