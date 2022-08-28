using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Deszz.Undebugger.UI.Layout
{
    public struct RectChild
    {
        public float MinWidth => LayoutElement == null ? DefaultMinWidth : LayoutElement.minWidth;
        public float MinHeight => LayoutElement == null ? DefaultMinHeight : LayoutElement.minHeight;

        public RectTransform Rect;
        public LayoutElement LayoutElement;
        public float DefaultMinWidth;
        public float DefaultMinHeight;
    }

    public static class LayoutUtility
    {
        public static void SetLayoutDirty(Transform transform, LayoutDirtyFlag flag = LayoutDirtyFlag.All)
        {
            var master = transform.GetComponentInParent<LayoutMaster>();
            if (master != null)
            {
                master.SetDirty(flag);
            }
        }

        public static void SetLayoutDirtyAndForceUpdate(Transform transform, LayoutDirtyFlag flag = LayoutDirtyFlag.All)
        {
            var master = transform.GetComponentInParent<LayoutMaster>();
            if (master != null)
            {
                master.SetDirty(flag);
                master.ForceRefresh();
            }
        }

        public static bool TryGetChildData(RectTransform rect, out RectChild child)
        {
            GetDimensions(rect, out var layoutElement, out float minWidth, out float minHeight);
            if (layoutElement != null && layoutElement.ignoreLayout)
            {
                child = default;
                return false;
            }

            child = new RectChild() { Rect = rect, LayoutElement = layoutElement, DefaultMinHeight = minHeight, DefaultMinWidth = minWidth };
            return true;
        }

        public static List<RectChild> FindChildrens(Transform self)
        {
            var rects = new List<RectChild>();

            for (int i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                var rect = child.GetComponent<RectTransform>();
                if (rect == null)
                {
                    continue;
                }

                GetDimensions(rect, out var layoutElement, out float minWidth, out float minHeight);
                if (layoutElement != null && layoutElement.ignoreLayout)
                {
                    continue;
                }

                rects.Add(new RectChild() { Rect = rect, LayoutElement = layoutElement, DefaultMinHeight = minHeight, DefaultMinWidth = minWidth });
            }

            return rects;
        }

        public static void GetDimensions(RectTransform rect, out LayoutElement layout, out float defaultMinWidth, out float defaultMinHegiht)
        {
            defaultMinWidth = rect.rect.width;
            defaultMinHegiht = rect.rect.height;

            layout = rect.GetComponent<LayoutElement>();
            if (layout != null)
            {
                if (layout.minWidth != -1)
                {
                    defaultMinWidth = layout.minWidth;
                }

                if (layout.minHeight != -1)
                {
                    defaultMinHegiht = layout.minHeight;
                }
            }
        }
    }
}
