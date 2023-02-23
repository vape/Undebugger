using UnityEngine;

namespace Undebugger.UI.Layout
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class UVerticalList : ULayoutNode
    {
        [SerializeField]
        private RectOffset padding;

        protected override void OnLayout()
        {
            base.OnLayout();

            var offset = 0f;
            var width = Self.rect.width;

            for (int i = 0; i < childrens.Count; ++i)
            {
                var c = childrens[i];
                var x = 0;
                var y = offset;
                var h = c.MinHeight;
                var w = width;

                childrens[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x, w);
                childrens[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y, h);

                offset += h;
            }

            Self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, offset + padding.bottom);
        }
    }
}
