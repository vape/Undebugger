using UnityEngine;

namespace Undebugger.UI.Layout
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class UHorizontalList : ULayoutNode
    {
        [SerializeField]
        private RectOffset padding;

        protected override void OnLayout()
        {
            base.OnLayout();

            var offset = 0f;
            var height = Self.rect.height;

            for (int i = 0; i < childrens.Count; ++i)
            {
                var c = childrens[i];
                var x = offset;
                var y = 0;
                var h = height;
                var w = c.MinWidth;

                childrens[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, x, w);
                childrens[i].Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y, h);

                offset += w;
            }

            Self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, offset + padding.right);
        }
    }
}
