using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Deszz.Undebugger.UI.Layout
{
    public struct RectChild
    {
        public RectTransform Rect;
        public RectDimensions Dimensions;
    }

    public struct RectDimensions
    {
        public bool Ignored;
        public float MinWidth;
        public float MinHeight;
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

                var dimensions = GetDimensions(rect);
                if (dimensions.Ignored)
                {
                    continue;
                }

                rects.Add(new RectChild() { Rect = rect, Dimensions = dimensions });
            }

            return rects;
        }

        public static RectDimensions GetDimensions(RectTransform rect)
        {
            var minw = rect.rect.width;
            var minh = rect.rect.height;
            var w = rect.rect.width;
            var h = rect.rect.height;
            var ignored = false;

            var layout = rect.GetComponent<LayoutElement>();
            if (layout != null)
            {
                ignored = layout.ignoreLayout;

                minw = layout.minWidth == -1 ? w : layout.minWidth;
                minh = layout.minHeight == -1 ? h : layout.minHeight;

                w = Mathf.Max(minw, w);
                h = Mathf.Max(minh, h);
            }

            return new RectDimensions()
            {
                MinWidth = minw,
                MinHeight = minh,
                Ignored = ignored
            };
        }
    }
}
