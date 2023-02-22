using System.Collections.Generic;

namespace Undebugger.UI.Layout
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class UBasicLayout : ULayoutRoot
    {
        protected List<RectChild> childrens;

        protected override void OnDisable()
        {
            if (childrens != null)
            {
                UListPool<RectChild>.Return(childrens);
                childrens = null;
            }

            base.OnDisable();
        }

        protected override void OnHierarchyRebuild()
        {
            base.OnHierarchyRebuild();

            if (childrens == null)
            {
                childrens = UListPool<RectChild>.Get(transform.childCount);
            }

            childrens.Clear();
            ULayoutHelper.FindRectChildrens(transform, childrens);
        }
    }
}
