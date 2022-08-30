using UnityEngine;
using UnityEngine.EventSystems;

namespace Deszz.Undebugger.UI.Windows
{
    public class WindowResizeHandle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
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
        private RectTransform root;

        private Canvas canvas;
        private RectTransform rect;
        private bool dragging;
        private Vector3 dragStartMousePosition;
        private Vector3 dragStartRectPosition;
        private Rect dragStartRect;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(Rect, eventData.position, canvas.worldCamera, out dragStartMousePosition);
            dragStartRectPosition = root.position;
            dragStartRect = root.rect;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragging)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(Rect, eventData.position, canvas.worldCamera, out var worldMousePosition);
                var delta = worldMousePosition - dragStartMousePosition;
                var sizeDelta = root.InverseTransformVector(delta);

                root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dragStartRect.width + sizeDelta.x);
                root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dragStartRect.height - sizeDelta.y);
                root.transform.position = dragStartRectPosition + delta * 0.5f;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
        }
    }
}
