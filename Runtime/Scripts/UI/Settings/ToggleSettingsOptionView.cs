using Undebugger.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Settings
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class ToggleSettingsOptionView : SettingsOptionView
    {
        [SerializeField]
        private UndebuggerToggle toggle;
        [SerializeField]
        private Text text;

        private ToggleSettingsOption option;

        public void Setup(ToggleSettingsOption option)
        {
            this.option = option;

            text.text = option.Name;
            toggle.IsOn = option.GetValue();
        }

        public void OnValueChanged(bool value)
        {
            option.SetValue(value);
            toggle.IsOn = option.GetValue();
        }

        public override void Refresh()
        {
            base.Refresh();

            if (option != null)
            {
                toggle.IsOn = option.GetValue();
            }
        }
    }
}
