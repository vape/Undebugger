using System;
using System.Collections.Generic;
using UnityEngine;

namespace Undebugger.UI.Layout
{
    [Flags]
    internal enum ULayoutDirtyFlag : short
    {
        None = 0,
        Layout = 1,
        Hierarchy = 2,
        All = Layout | Hierarchy
    }

    internal static class ULayoutHelper
    {
        public static void SetDirty(Transform source, ULayoutDirtyFlag flag)
        {
            if (!TrySetDirtyForParentRoot(source, flag))
            {
                TrySetDirtyForChildRoots(source, flag);
            }
        }

        private static bool TrySetDirtyForParentRoot(Transform source, ULayoutDirtyFlag flag)
        {
            ULayoutRoot root = null;

            while (source != null)
            {
                if (source.TryGetComponent<ULayoutRoot>(out var _root) && _root.isActiveAndEnabled)
                {
                    root = _root;
                }

                source = source.parent;
            }

            if (root != null)
            {
                root.SetDirty(flag);
                return true;
            }

            return false;
        }

        private static void TrySetDirtyForChildRoots(Transform source, ULayoutDirtyFlag flag)
        {
            if (source.TryGetComponent<ULayoutRoot>(out var layoutRoot))
            {
                layoutRoot.SetDirty(flag);
                return;
            }

            var childsCount = source.childCount;

            for (int i = 0; i < childsCount; ++i)
            {
                TrySetDirtyForChildRoots(source.GetChild(i), flag);
            }
        }

        public static void FindRectChildrens(Transform root, List<RectChild> list)
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
    }
}
