using UnityEngine;
using UnityEngine.EventSystems;

namespace Deszz.Undebugger.UI.Windows
{
    public class WindowResizeHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        [SerializeField]
        private Vector2 direction = Vector2.one;

        private bool dragging;
        private Vector3 startMousePosition;
        private Vector3 startRootPosition;
        private Rect startRootRect;
        private RectTransform rect;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(Rect, eventData.position, eventData.pressEventCamera, out startMousePosition))
            {
                dragging = true;
                startRootPosition = root.position;
                startRootRect = root.rect;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragging && RectTransformUtility.ScreenPointToWorldPointInRectangle(Rect, eventData.position, eventData.pressEventCamera, out var currentMousePosition))
            {
                var worldDelta = currentMousePosition - startMousePosition;
                var localDelta = root.InverseTransformVector(worldDelta);

                root.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, startRootRect.width + localDelta.x * direction.x);
                root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, startRootRect.height - localDelta.y * direction.y);
                root.position = startRootPosition + Vector3.Scale(worldDelta, root.pivot);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
        }
    }
}
