using Undebugger.Services.Log;
using Undebugger.UI.Controls;
using UnityEngine;

namespace Undebugger.UI.Menu.Logs
{
    public class LogGroupView : GroupView
    {
        public override string GroupName => "Log";

        [SerializeField]
        private ToggleButton infoToggle;
        [SerializeField]
        private ToggleButton warningToggle;
        [SerializeField]
        private ToggleButton errorToggle;
        [SerializeField]
        private MessagesListView messagesList;

        public override void UsePool(MenuPool pool)
        {
            base.UsePool(pool);

            messagesList.UsePool(pool);
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void Toggle(int mask)
        {
            var currentMask = (int)messagesList.GetMask();
            if ((currentMask & mask) > 0)
            {
                currentMask &= ~mask;
            }
            else
            {
                currentMask |= mask;
            }

            messagesList.SetMask((LogTypeMask)currentMask);
            Refresh();
        }

        private void Refresh()
        {
            var mask = messagesList.GetMask();

            infoToggle.Selected = (mask & LogTypeMask.Info) > 0;
            warningToggle.Selected = (mask & LogTypeMask.Warning) > 0;
            errorToggle.Selected = (mask & LogTypeMask.Error) > 0;
        }
    }
}
