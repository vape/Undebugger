using System;
using System.Collections.Generic;
using UnityEngine;

namespace Deszz.Undebugger.UI.Layout
{
    [RequireComponent(typeof(RectTransform))]
    public class FlexibleGrid : MonoBehaviour
    {
        [Flags]
        public enum AxisMask
        {
            None = 0,
            Horizontal = 1,
            Vertical = 2
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
        private AxisMask autoSize;

        private bool layouting;
        private GridElement[] layout;
        private Vector2 size;

        public void Layout()
        {
            if (layouting)
            {
                return;
            }

            layouting = true;

            var self = GetComponent<RectTransform>();
            var rects = LayoutUtility.FindChildrens(self);

            BuildLayout(rects, self);

            if ((autoSize & AxisMask.Vertical) > 0)
            {
                self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            }

            if ((autoSize & AxisMask.Horizontal) > 0)
            {
                self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            }

            for (int i = 0; i < rects.Count; ++i)
            {
                layout[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, layout[i].X, layout[i].W);
                layout[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, layout[i].Y, layout[i].H);
            }

            layouting = false;
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
                var extraWidth = maxWidth - currentRowWidth;
                var offset = 0f;
                
                if (extraWidth <= 0)
                {
                    return;
                }

                for (int i = first; i < last; ++i)
                {
                    var extraElementWidth = (layout[i].W / currentRowWidth) * extraWidth;

                    layout[i].H = currentRowHeight;
                    layout[i].W += extraElementWidth;
                    layout[i].X += offset;

                    offset += extraElementWidth;
                }

                currentRowWidth = maxWidth;
            }

            void nextRow()
            {
                currentTopOffset += currentRowHeight;
                currentElementsInRow = 0;
                currentRowWidth = 0;
                currentRowHeight = 0;
            }
            
            for (int i = 0; i < rects.Count; i++)
            {
                if (currentElementsInRow > 0 && currentRowWidth + rects[i].Dimensions.MinWidth > maxWidth)
                {
                    adjustRowDimensions(i, currentElementsInRow);
                    --i;
                    nextRow();
                    continue;
                }

                var height = rects[i].Dimensions.Height;
                if (height > currentRowHeight)
                {
                    currentRowHeight = height;
                }

                layout[i] = new GridElement()
                {
                    Rect = rects[i].Rect,
                    W = rects[i].Dimensions.MinWidth,
                    H = height,
                    X = padding.left + currentRowWidth,
                    Y = currentTopOffset
                };

                currentElementsInRow++;
                currentRowWidth += layout[i].W;
            }

            adjustRowDimensions(rects.Count, currentElementsInRow);

            size = new Vector2(maxWidth, currentTopOffset + currentRowHeight + padding.bottom);
        }
    }
}
