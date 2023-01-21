using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Layout
{
    [RequireComponent(typeof(RectTransform))]
    internal class FlexibleGrid : LayoutRoot
    {
        [Serializable]
        public struct RectSpacing
        {
            public float Horizontal;
            public float Vertical;
        }

        private struct GridElement
        {
            public RectTransform Rect;
            public float X;
            public float Y;
            public float W;
            public float H;
        }

        [SerializeField]
        private RectOffset padding;
        [SerializeField]
        private RectSpacing spacing;
        [SerializeField]
        private LayoutAxis autoSize;

        private GridElement[] layout;
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

            if ((autoSize & LayoutAxis.Vertical) > 0)
            {
                self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            }

            if ((autoSize & LayoutAxis.Horizontal) > 0)
            {
                self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            }

            for (int i = 0; i < childrens.Count; ++i)
            {
                layout[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, layout[i].X, layout[i].W);
                layout[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, layout[i].Y, layout[i].H);
            }

            OnLayoutChanged();
        }

        private void BuildLayout(List<RectChild> rects, RectTransform self)
        {
            if (layout == null || rects.Count < layout.Length)
            {
                layout = new GridElement[rects.Count];
            }

            var maxWidth = self.rect.width - padding.horizontal;
            var currentRowWidth = 0f;
            var currentRowHeight = 0f;
            var currentTopOffset = (float)padding.top;
            var currentElementsInRow = 0;

            void adjustRowDimensions(int last, int count)
            {
                var first = last - count;
                // add horizontal spacing here to compensate extra space added to the end of a row
                var extraWidth = maxWidth - currentRowWidth + spacing.Horizontal;
                var offset = 0f;
                
                if (extraWidth <= 0)
                {
                    return;
                }

                for (int i = first; i < last; ++i)
                {
                    var extraElementWidth = ((layout[i].W + spacing.Horizontal) / currentRowWidth) * extraWidth;

                    layout[i].H = currentRowHeight;
                    layout[i].W += extraElementWidth;
                    layout[i].X += offset;

                    offset += extraElementWidth;
                }

                currentRowWidth = maxWidth;
            }

            void nextRow()
            {
                currentTopOffset += currentRowHeight + spacing.Vertical;
                currentElementsInRow = 0;
                currentRowWidth = 0;
                currentRowHeight = 0;
            }
            
            for (int i = 0; i < rects.Count; i++)
            {
                if (currentElementsInRow > 0 && currentRowWidth + rects[i].MinWidth > maxWidth)
                {
                    adjustRowDimensions(i, currentElementsInRow);
                    --i;
                    nextRow();
                    continue;
                }

                var height = rects[i].MinHeight;
                if (height > currentRowHeight)
                {
                    currentRowHeight = height;
                }

                var width = rects[i].MinWidth;
                if (currentElementsInRow == 0 && width > maxWidth)
                {
                    width = maxWidth;
                }

                layout[i] = new GridElement()
                {
                    Rect = rects[i].Rect,
                    W = width,
                    H = height,
                    X = padding.left + currentRowWidth,
                    Y = currentTopOffset
                };

                currentElementsInRow++;
                currentRowWidth += layout[i].W + spacing.Horizontal;
            }

            adjustRowDimensions(rects.Count, currentElementsInRow);

            size = new Vector2(maxWidth, currentTopOffset + currentRowHeight + padding.bottom);
        }
    }
}
