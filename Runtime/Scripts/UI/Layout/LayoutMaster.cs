using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Deszz.Undebugger.UI.Layout
{
    [Flags]
    public enum LayoutDirtyFlag : uint
    {
        None = 0,
        Layout = 1,
        Hierarchy = 2,
        All = uint.MaxValue
    }

    internal class LayoutMaster : MonoBehaviour
    {
        private struct LayoutRootRecord
        {
            public LayoutRoot Root;
            public int Layer;
        }

        private Dictionary<int, List<LayoutRoot>> cache = new Dictionary<int, List<LayoutRoot>>();
        private LayoutDirtyFlag dirtyFlag;

        public void SetDirty(LayoutDirtyFlag flag = LayoutDirtyFlag.All)
        {
            dirtyFlag |= flag;
        }

        private void Update()
        {
            if (dirtyFlag != LayoutDirtyFlag.None)
            {
                if ((dirtyFlag & LayoutDirtyFlag.Hierarchy) != 0)
                {
                    ResetHierarchyCache();
                }

                if ((dirtyFlag & LayoutDirtyFlag.Layout) != 0)
                {
                    DoLayout();
                }

                dirtyFlag = LayoutDirtyFlag.None;
            }
        }

        public void ResetHierarchyCache()
        {
            if (cache != null)
            {
                foreach (var layout in GetLayoutRootsDownToTop())
                {
                    if (layout != null)
                    {
                        layout.ResetHierarchyCache();
                    }
                }

                cache = null;
            }
        }

        public void RebuildHierarchyCache()
        {
            ResetHierarchyCache();

            cache = new Dictionary<int, List<LayoutRoot>>();

            foreach (var record in FindLayoutRoots(transform))
            {
                if (!cache.TryGetValue(record.Layer, out var list))
                {
                    list = new List<LayoutRoot>();
                    cache.Add(record.Layer, list);
                }

                list.Add(record.Root);
            }

            foreach (var layout in GetLayoutRootsDownToTop())
            {
                layout.BuildHierarchyCache();
            }
        }

        private IEnumerable<LayoutRoot> GetLayoutRootsDownToTop()
        {
            foreach (var kv in cache.OrderByDescending(kv => kv.Key))
            {
                foreach (var root in kv.Value)
                {
                    yield return root;
                }
            }
        }

        private IEnumerable<LayoutRootRecord> FindLayoutRoots(Transform root, int layer = 0)
        {
            for (int i = 0; i < root.childCount; ++i)
            {
                var child = root.GetChild(i);

                if (child.TryGetComponent<LayoutRoot>(out var layout))
                {
                    yield return new LayoutRootRecord() { Layer = layer, Root = layout };
                }

                foreach (var record in FindLayoutRoots(child, layer: layer + 1))
                {
                    yield return record;
                }
            }
        }

        public void DoLayout()
        {
            if (cache == null)
            {
                RebuildHierarchyCache();
            }

            foreach (var layout in GetLayoutRootsDownToTop())
            {
                layout.DoLayout();
            }
        }
    }
}
