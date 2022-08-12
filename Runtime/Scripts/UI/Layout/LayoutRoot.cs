using UnityEngine;

namespace Deszz.Undebugger.UI.Layout
{
    internal abstract class LayoutRoot : MonoBehaviour
    {
        public virtual void ResetHierarchyCache()
        { }

        public virtual void BuildHierarchyCache()
        { }

        public virtual void DoLayout()
        { }
    }
}
