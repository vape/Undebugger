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
            while (source != null)
            {
                if (source.TryGetComponent<ULayoutMaster>(out var master) && master.isActiveAndEnabled)
                {
                    master.SetDirty(flag);
                    return;
                }

                source = source.parent;
            }
        }
    }
}
