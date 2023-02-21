using Undebugger.Model;
using UnityEngine;

namespace Undebugger.UI.Menu
{
    internal abstract class GroupView : MonoBehaviour, IPoolable, IPoolHandler
    {
        public abstract string GroupName { get; }

        protected MenuPool pool;

        public virtual void Load(MenuModel menuModel)
        { }

        public virtual void UsePool(MenuPool pool)
        {
            this.pool = pool;
        }

        public virtual void AddingToPool()
        { }
    }
}
