using System;
using UnityEngine;

namespace Undebugger.UI.Layout
{
    internal abstract class LayoutRoot : MonoBehaviour
    {
        public event Action LayoutChanged;

        [SerializeField]
        protected LayoutAxis expand = LayoutAxis.None;

        protected RectTransform parent;
        protected RectTransform self;

        public virtual void ResetHierarchyCache()
        {
            parent = null;
            self = null;
        }

        public virtual void BuildHierarchyCache()
        {
            self = GetComponent<RectTransform>();
            parent = self.parent.GetComponent<RectTransform>();
        }

        public virtual void DoLayout()
        {
            if (self == null)
            {
                BuildHierarchyCache();
            }

            if (parent != null)
            {
                if ((expand & LayoutAxis.Horizontal) != 0)
                {
                    self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parent.rect.size.x);
                }

                if ((expand & LayoutAxis.Vertical) != 0)
                {
                    self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parent.rect.size.y);
                }
            }
        }

        protected void OnLayoutChanged()
        {
            LayoutChanged?.Invoke();
        }
    }
}
