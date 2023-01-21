using UnityEngine;

namespace Undebugger.UI.Layout
{
    [RequireComponent(typeof(LayoutMaster))]
    internal class LayoutMasterAutolayout : MonoBehaviour
    {
        [SerializeField]
        private LayoutMaster master;

        protected void OnRectTransformDimensionsChange()
        {
            master.SetDirty(LayoutDirtyFlag.Layout);
            master.ForceRefresh();
        }
    }
}
