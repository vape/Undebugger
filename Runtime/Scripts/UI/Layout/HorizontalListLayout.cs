using System.Collections.Generic;
using UnityEngine;

namespace Undebugger.UI.Layout
{
    [RequireComponent(typeof(RectTransform))]
    internal class HorizontalListLayout : LayoutRoot 
    {
        private struct ListElement
        {
            public RectTransform Rect;
            public float X;
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
                layout[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, layout[i].X, layout[i].W);
                layout[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, layout[i].H);
            }

            self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);

            OnLayoutChanged();
        }

        private void BuildLayout(List<RectChild> rects, RectTransform self)
        {
            if (layout == null || layout.Length < rects.Count)
            {
                layout = new ListElement[rects.Count];
            }

            var height = self.rect.height;
            var offset = 0f;

            for (int i = 0; i < rects.Count; ++i)
            {
                layout[i] = new ListElement()
                {
                    W = Mathf.Max(rects[i].Rect.rect.width, rects[i].MinWidth),
                    H = height,
                    X = offset,
                    Rect = rects[i].Rect
                };

                offset += layout[i].W;
            }

            size = new Vector2(offset, height);
        }
    }
}
