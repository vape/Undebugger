using UnityEngine;
using UnityEngine.EventSystems;

namespace Deszz.Undebugger.UI.Windows
{
    [RequireComponent(typeof(RectTransform))]
    public class WindowDragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform Rect
        {
            get
            {
                if (rect == null)
                {
                    rect = GetComponent<RectTransform>();
                }

                return rect;
            }
        }

        [SerializeField]
        protected RectTransform root;


        private bool dragging;
        private Vector3 startMousePosition;
        private Vector3 startRootPosition;
        private RectTransform rect;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(Rect, eventData.position, eventData.pressEventCamera, out startMousePosition))
            {
                dragging = true;
                startRootPosition = root.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragging && RectTransformUtility.ScreenPointToWorldPointInRectangle(Rect, eventData.position, eventData.pressEventCamera, out var currentMousePosition))
            {
                root.position = startRootPosition + currentMousePosition - startMousePosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
        }
    }
}
