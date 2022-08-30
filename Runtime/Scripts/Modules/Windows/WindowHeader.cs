using UnityEngine;

namespace Deszz.Undebugger.UI.Windows
{
    [RequireComponent(typeof(RectTransform))]
    public class WindowHeader : MonoBehaviour, IWindowButtonsContainer
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

        public float Height
        {
            get
            {
                return height;
            }
        }

        [SerializeField]
        private RectTransform buttonsContainer;

        private RectTransform rect;
        private float height;

        private void Awake()
        {
            height = Rect.rect.height;
        }

        public void AttachWindowButtons(WindowControlButton[] buttons)
        {
            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].transform.SetParent(buttonsContainer);
            }
        }

        public void DetachWindowButtons()
        {
            for (int i = 0; i < buttonsContainer.childCount; ++i)
            {
                Destroy(buttonsContainer.GetChild(i).gameObject);
            }
        }
    }
}
