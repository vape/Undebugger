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

            SetDirtyLayout();
        }

        private void RefreshText()
        {
            title.text = driver.Title;
            text.text = driver.Text;
        }

        private void TextDimensionsChanged()
        {
            SetDirtyLayout();
        }

        private void OnRectTransformDimensionsChange()
        {
            SetDirtyLayout();
        }

        private void SetDirtyLayout()
        {
            Layout.LayoutUtility.SetLayoutDirty(transform, LayoutDirtyFlag.Layout);
        }
    }
}
