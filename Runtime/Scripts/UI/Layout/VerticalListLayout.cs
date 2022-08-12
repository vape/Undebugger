using System.Collections.Generic;
using UnityEngine;

namespace Deszz.Undebugger.UI.Layout
{
    [RequireComponent(typeof(RectTransform))]
    public class VerticalListLayout : MonoBehaviour
    {
        private struct ListElement
        {
            public RectTransform Rect;
            public float Y;
            public float W;
            public float H;
        }

        private bool layouting;
        private ListElement[] layout;
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

            for (int i = 0; i < rects.Count; ++i)
            {
                layout[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, layout[i].W);
                layout[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, layout[i].Y, layout[i].H);
            }

            self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

            layouting = false;
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
                    H = rects[i].Dimensions.Height,
                    Y = offset,
                    Rect = rects[i].Rect
                };

                offset += layout[i].H;
            }

            size = new Vector2(width, offset);
        }
    }
}
