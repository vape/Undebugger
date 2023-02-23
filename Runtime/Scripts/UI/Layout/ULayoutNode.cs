using System.Collections.Generic;
using UnityEngine;

namespace Undebugger.UI.Layout
{
    [RequireComponent(typeof(RectTransform))]
    internal abstract class ULayoutNode : MonoBehaviour, IULayoutNode
    {
        public static void FindChildrens(Transform root, List<RectChild> list)
        {
            var count = root.childCount;

            for (int i = 0; i < count; ++i)
            {
                var child = root.GetChild(i);
                if (child.TryGetComponent<RectTransform>(out var rect))
                {
                    child.TryGetComponent<IULayoutElement>(out var layoutElement);
                    if (layoutElement != null && layoutElement.Ignore)
                    {
                        continue;
                    }

                    list.Add(new RectChild()
                    {
                        Rect = rect,
                        LayoutElement = layoutElement
                    });
                }
            }
        }

        public bool IsActive
        {
            get
            {
                return enabled && gameObject.activeInHierarchy && gameObject.activeSelf;
            }
        }

        protected RectTransform Self
        {
            get
            {
                if (self == null)
                {
                    TryGetComponent(out self);
                }

                return self;
            }
        }

        private RectTransform self;
        protected List<RectChild> childrens;
        protected bool resizing;

        protected virtual void OnDisable()
        {
            if (childrens != null)
            {
                UListPool<RectChild>.Return(childrens);
                childrens = null;
            }
        }

        public virtual void OnHierarchyRebuild()
        {
            if (childrens == null)
            {
                childrens = UListPool<RectChild>.Get(transform.childCount);
            }

            childrens.Clear();
            FindChildrens(transform, childrens);
        }

        public virtual void OnLayoutRebuild()
        {
            resizing = true;

            OnLayout();

            resizing = false;
        }

        protected virtual void OnLayout()
        { }

        protected virtual void OnRectTransformDimensionsChange()
        {
            if (!resizing && IsActive)
            {
                ULayoutHelper.SetDirty(Self, ULayoutDirtyFlag.Layout);
            }
        }
    }
}
