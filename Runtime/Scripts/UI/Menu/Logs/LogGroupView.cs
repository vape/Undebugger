using Undebugger.Services.Log;
using Undebugger.Services.UI;
using Undebugger.UI.Controls;
using Undebugger.UI.Settings;
using UnityEngine;

namespace Undebugger.UI.Menu.Logs
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class LogGroupView : GroupView
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
        [SerializeField]
        private SettingsMenu settings;

        public override void UsePool(MenuPool pool)
        {
            base.UsePool(pool);

            messagesList.UsePool(pool);
        }

        private void Awake()
        {
            settings.Setup(new ToggleSettingsOption()
            {
                Name = "Error notifications",
                GetValue = () => UIService.Instance.ErrorNotificationWidgetEnabled,
                SetValue = (value) => UIService.Instance.ErrorNotificationWidgetEnabled = value
            });
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
