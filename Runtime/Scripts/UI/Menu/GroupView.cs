using Undebugger.Model;
using UnityEngine;

namespace Undebugger.UI.Menu
{
    public abstract class GroupView : MonoBehaviour
    {
        public abstract string GroupName { get; }

        public virtual void Load(MenuModel menuModel, MenuContext menuContext)
        { }
    }
}
