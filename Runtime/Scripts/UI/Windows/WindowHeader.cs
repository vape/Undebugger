using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Deszz.Undebugger.UI.Windows
{
    [RequireComponent(typeof(RectTransform))]
    public class WindowHeader : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IWindowButtonsContainer
    {
        public bool Draggable
        { get; set; }

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
        [SerializeField]
        private RectTransform buttonsContainer;

        private Canvas canvas;
        private RectTransform rect;
        private bool dragging;
        private Vector3 dragStartMousePosition;
        private Vector3 dragStartRootPosition;

        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Draggable)
            {
                return;
            }

            dragging = true;

            RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, canvas.worldCamera, out dragStartMousePosition);
            dragStartRootPosition = root.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragging)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, canvas.worldCamera, out var worldMousePosition); 
                var delta = worldMousePosition - dragStartMousePosition;

                root.position = dragStartRootPosition + delta;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
        }

        public void AddWindowButtons(WindowHeaderButton[] buttons)
        {
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].transform.SetParent(buttonsContainer);
            }
        }

        public void RemoveAllWindowButtons()
        {
            for (int i = 0; i < buttonsContainer.childCount; ++i)
            {
                Destroy(buttonsContainer.GetChild(i).gameObject);
            }
        }
    }
}
