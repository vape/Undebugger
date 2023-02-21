using UnityEngine;

namespace Undebugger.UI.Layout
{
    internal struct RectChild
    {
        public float MinWidth => LayoutElement != null && LayoutElement.MinWidth >= 0 ? LayoutElement.MinWidth : Rect.sizeDelta.x;
        public float MinHeight => LayoutElement != null && LayoutElement.MinHeight >= 0 ? LayoutElement.MinHeight : Rect.sizeDelta.y;

        public RectTransform Rect;
        public IULayoutElement LayoutElement;
    }
}
