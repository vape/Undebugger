using System;
using System.Collections.Generic;
using UnityEngine;

namespace Undebugger.UI.Layout
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class UFlexGrid : ULayoutNode
    {
        [Serializable]
        public struct RectSpacing
        {
            public float Horizontal;
            public float Vertical;
        }

        public struct GridElement
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

        protected override void OnLayout()
        {
            base.OnLayout();

            var elements = UListPool<GridElement>.Get(childrens.Count);
            var size = CreateLayout(elements, childrens, padding, spacing, Self);

            Self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

            for (int i = 0; i < elements.Count; ++i)
            {
                elements[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, elements[i].X, elements[i].W);
                elements[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, elements[i].Y, elements[i].H);
            }

            UListPool<GridElement>.Return(elements);
        }

        public static Vector2 CreateLayout(List<GridElement> layout, List<RectChild> rects, RectOffset padding, RectSpacing spacing, RectTransform self)
        {
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

                    var gridElement = layout[i];
                    gridElement.H = currentRowHeight;
                    gridElement.W += extraElementWidth;
                    gridElement.X += offset;
                    layout[i] = gridElement;

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

                layout.Add(new GridElement()
                {
                    Rect = rects[i].Rect,
                    W = width,
                    H = height,
                    X = padding.left + currentRowWidth,
                    Y = currentTopOffset
                });

                currentElementsInRow++;
                currentRowWidth += layout[i].W + spacing.Horizontal;
            }

            adjustRowDimensions(rects.Count, currentElementsInRow);

            return new Vector2(maxWidth, currentTopOffset + currentRowHeight + padding.bottom);
        }
    }
}
