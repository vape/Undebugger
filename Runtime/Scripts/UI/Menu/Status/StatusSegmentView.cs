using Undebugger.Model.Status;
using Undebugger.UI.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Status
{
    public class StatusSegmentView : MonoBehaviour
    {
        [SerializeField]
        private Text title;
        [SerializeField]
        private Text text;
        [SerializeField]
        private LayoutRoot layout;
        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        private Button foldButton;
        [SerializeField]
        private Button unfoldButton;

        private bool folded;
        private IStatusSegmentDriver driver;

        private void OnEnable()
        {
            layout.LayoutChanged += LayoutChangedHandler;
        }

        private void OnDisable()
        {
            layout.LayoutChanged -= LayoutChangedHandler;
        }

        private void OnDestroy()
        {
            Deinit();
        }

        private void Deinit()
        {
            if (driver != null)
            {
                driver.Changed -= DriverChangedHandler;
            }
        }

        private void DriverChangedHandler()
        {
            RefreshText();
        }

        private void LayoutChangedHandler()
        {
            layoutElement.minHeight = GetComponent<RectTransform>().rect.size.y;
        }

        public void Init(IStatusSegmentDriver driver)
        {
            Deinit();

            this.driver = driver;
            this.driver.Changed += DriverChangedHandler;

            RefreshText();

            var dimensionsTracker = text.gameObject.AddComponent<RectTransformDimensionsTracker>();
            dimensionsTracker.DimensionsChanged += TextDimensionsChanged;

            SetFoldout(false);
        }

        public void SetFoldout(bool value)
        {
            folded = value;

            if (folded)
            {
                text.gameObject.SetActive(false);
                SetLayoutHierarchyDirty();
            }
            else
            {
                text.gameObject.SetActive(true);
                SetLayoutHierarchyDirty();
            }

            foldButton.gameObject.SetActive(!folded);
            unfoldButton.gameObject.SetActive(folded);
        }

        private void RefreshText()
        {
            title.text = driver.Title;
            text.text = driver.Text;
        }

        private void TextDimensionsChanged()
        {
            SetLayoutDirty();
        }

        private void OnRectTransformDimensionsChange()
        {
            SetLayoutDirty();
        }

        private void SetLayoutDirty()
        {
            Layout.LayoutUtility.SetLayoutDirty(transform, LayoutDirtyFlag.Layout);
        }

        private void SetLayoutHierarchyDirty()
        {
            Layout.LayoutUtility.SetLayoutDirty(transform, LayoutDirtyFlag.Layout | LayoutDirtyFlag.Hierarchy);
        }
    }
}
