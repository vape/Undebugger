using System.Collections.Generic;
using UnityEngine;

namespace Undebugger.UI.Layout
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    [RequireComponent(typeof(RectTransform))]
    internal class ULayoutRoot : MonoBehaviour
    {
        protected static void BuildHierarchy(Transform root, List<List<ULayoutRoot>> layers)
        {
            var childCount = root.childCount;
            List<ULayoutRoot> roots = null;

            while (childCount-- > 0)
            {
                var child = root.GetChild(childCount);
                if (child.TryGetComponent<ULayoutRoot>(out var childLayoutRoot))
                {
                    if (roots == null)
                    {
                        roots = UListPool<ULayoutRoot>.Get(capacity: 8);
                        layers.Add(roots);
                    }

                    childLayoutRoot.OnHierarchyRebuild();
                    roots.Add(childLayoutRoot);
                }

                BuildHierarchy(child, layers);
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

        protected ULayoutDirtyFlag dirtyFlag;
        
        private RectTransform self;
        private List<List<ULayoutRoot>> hierarchy;

        protected virtual void OnDisable()
        {
            if (hierarchy != null)
            {
                ClearHierarchy();
                UListPool<List<ULayoutRoot>>.Return(hierarchy);
                hierarchy = null;
            }
        }
        
        protected virtual void Update()
        {
            ProcessDirtyFlag();
        }

        public void SetDirty(ULayoutDirtyFlag flag)
        {
            dirtyFlag |= flag;
        }

        public void ForceProcess()
        {
            ProcessDirtyFlag();
        }

        protected virtual void OnHierarchyRebuild()
        { }

        protected virtual void OnLayout()
        { }

        protected virtual void ProcessDirtyFlag()
        {
            if (dirtyFlag != ULayoutDirtyFlag.None)
            {
                if ((dirtyFlag & ULayoutDirtyFlag.Hierarchy) != 0)
                {
                    RebuildHierarchy();
                    OnHierarchyRebuild();
                }

                if ((dirtyFlag & ULayoutDirtyFlag.Layout) != 0 && hierarchy != null)
                {
                    RebuildLayout();
                }

                dirtyFlag = ULayoutDirtyFlag.None;
            }
        }

        private void RebuildLayout()
        {
            for (int i = hierarchy.Count - 1; i >= 0; --i)
            {
                for (int j = 0; j < hierarchy[i].Count; ++j)
                {
                    hierarchy[i][j].OnLayout();
                }
            }

            OnLayout();
        }

        private void ClearHierarchy()
        {
            for (int i = 0; i < hierarchy.Count; ++i)
            {
                UListPool<ULayoutRoot>.Return(hierarchy[i]);
            }

            hierarchy.Clear();
        }

        private void RebuildHierarchy()
        {
            if (hierarchy == null)
            {
                hierarchy = UListPool<List<ULayoutRoot>>.Get(16);
            }
            else
            {
                ClearHierarchy();
            }

            BuildHierarchy(transform, hierarchy);
        }
    }
}
