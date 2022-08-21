using System.Collections.Generic;
using UnityEngine;

namespace Deszz.Undebugger.UI.Layout
{
    [RequireComponent(typeof(RectTransform))]
    internal class VerticalListLayout : LayoutRoot
    {
        private struct ListElement
        {
            public RectTransform Rect;
            public float Y;
            public float W;
            public float H;
        }

        private ListElement[] layout;
        private Vector2 size;
        private List<RectChild> childrens;

        public override void BuildHierarchyCache()
        {
            base.BuildHierarchyCache();

            childrens = LayoutUtility.FindChildrens(self);
        }

        public override void ResetHierarchyCache()
        {
            base.ResetHierarchyCache();

            childrens = null;
        }

        public override void DoLayout()
        {
            base.DoLayout();

            BuildLayout(childrens, self);

            for (int i = 0; i < childrens.Count; ++i)
            {
                layout[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, layout[i].W);
                layout[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, layout[i].Y, layout[i].H);
            }

            self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

            OnLayoutChanged();
        }

        private void BuildLayout(List<RectChild> rects, RectTransform self)
        {
            if (layout == null || layout.Length < rects.Count)
            {
                layout = new ListElement[rects.Count];
            }

            var width = self.rect.width;
            var offset = 0f;

            for (int i = 0; i < rects.Count; ++i)
            {
                layout[i] = new ListElement()
                {
                    W = width,
                    H = Mathf.Max(rects[i].Rect.rect.height, rects[i].MinHeight),
                    Y = offset,
                    Rect = rects[i].Rect
                };

                offset += layout[i].H;
            }

            size = new Vector2(width, offset);
        }
    }
}
