using System;
using System.Collections.Generic;
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
        protected List<ILayoutResizeHandler> resizeHandlers;

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

            RefreshResizeHandlers();
            OnBeginResize();

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

            OnEndResize();
        }

        protected void OnBeginResize()
        {
            for (int i = 0; i < resizeHandlers.Count; i++)
            {
                resizeHandlers[i].OnBeforeSizeChanged();
            }
        }

        protected void OnEndResize()
        {
            for (int i = 0; i < resizeHandlers.Count; i++)
            {
                resizeHandlers[i].OnAfterSizeChanged();
            }
        }

        protected void RefreshResizeHandlers()
        {
            if (resizeHandlers == null)
            {
                resizeHandlers = new List<ILayoutResizeHandler>(capacity: 2);
            }

            self.GetComponents(resizeHandlers);
        }

        protected void OnLayoutChanged()
        {
            LayoutChanged?.Invoke();
        }
    }
}
