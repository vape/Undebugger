using System.Collections.Generic;
using UnityEngine;

namespace Undebugger.UI.Layout
{
    internal class ULayoutMaster : MonoBehaviour
    {
        protected static void BuildHierarchy(Transform root, List<List<IULayoutNode>> layers)
        {
            var childCount = root.childCount;
            List<IULayoutNode> nodes = null;

            while (childCount-- > 0)
            {
                var child = root.GetChild(childCount);

                if (child.TryGetComponent<IULayoutNode>(out var childNode))
                {
                    if (nodes == null)
                    {
                        nodes = UListPool<IULayoutNode>.Get(capacity: 8);
                        layers.Add(nodes);
                    }

                    childNode.OnHierarchyRebuild();
                    nodes.Add(childNode);
                }

                if (!child.TryGetComponent<ULayoutMaster>(out _))
                {
                    BuildHierarchy(child, layers);
                }
            }
        }

        private ULayoutDirtyFlag dirtyFlag;
        private List<List<IULayoutNode>> hierarchy;

        protected virtual void OnDisable()
        {
            if (hierarchy != null)
            {
                ClearHierarchy();
                UListPool<List<IULayoutNode>>.Return(hierarchy);
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

        protected virtual void OnRectTransformDimensionsChange()
        {
            if (enabled && gameObject.activeInHierarchy && gameObject.activeSelf)
            {
                SetDirty(ULayoutDirtyFlag.Layout);
            }
        }

        protected virtual void ProcessDirtyFlag()
        {
            if (dirtyFlag != ULayoutDirtyFlag.None)
            {
                var _flag = dirtyFlag;
                dirtyFlag = ULayoutDirtyFlag.None;

                if ((_flag & ULayoutDirtyFlag.Hierarchy) != 0 || hierarchy == null)
                {
                    RebuildHierarchy();
                }

                if ((_flag & ULayoutDirtyFlag.Layout) != 0)
                {
                    RebuildLayout();
                }
            }
        }

        private void RebuildLayout()
        {
            for (int i = hierarchy.Count - 1; i >= 0; --i)
            {
                for (int j = 0; j < hierarchy[i].Count; ++j)
                {
                    if (hierarchy[i][j].IsActive)
                    {
                        hierarchy[i][j].OnLayoutRebuild();
                    }
                }
            }
        }

        private void ClearHierarchy()
        {
            for (int i = 0; i < hierarchy.Count; ++i)
            {
                UListPool<IULayoutNode>.Return(hierarchy[i]);
            }

            hierarchy.Clear();
        }

        private void RebuildHierarchy()
        {
            if (hierarchy == null)
            {
                hierarchy = UListPool<List<IULayoutNode>>.Get(16);
            }
            else
            {
                ClearHierarchy();
            }

            if (TryGetComponent<IULayoutNode>(out var selfLayoutNode))
            {
                var list = UListPool<IULayoutNode>.GetExact(1);
                list.Add(selfLayoutNode);
            }

            BuildHierarchy(transform, hierarchy);
        }
    }
}
