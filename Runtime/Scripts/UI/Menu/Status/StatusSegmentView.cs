using Deszz.Undebugger.Model.Status;
using Deszz.Undebugger.UI.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace Deszz.Undebugger.UI.Menu.Status
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

        private StatusSegmentModel model;

        private void OnEnable()
        {
            layout.LayoutChanged += LayoutChangedHandler;
        }

        private void OnDisable()
        {
            layout.LayoutChanged -= LayoutChangedHandler;
        }

        private void LayoutChangedHandler()
        {
            layoutElement.minHeight = GetComponent<RectTransform>().rect.size.y;
        }

        public void Init(StatusSegmentModel model)
        {
            this.model = model;

            title.text = model.Title;
            text.text = model.Text;

            var dimensionsTracker = text.gameObject.AddComponent<RectTransformDimensionsTracker>();
            dimensionsTracker.DimensionsChanged += TextDimensionsChanged;

            SetDirtyLayout();
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
