using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Settings
{
    internal class ToggleSettingsOptionView : SettingsOptionView
    {
        [SerializeField]
        private Toggle toggle;
        [SerializeField]
        private Text text;

        private ToggleSettingsOption option;

        public void Setup(ToggleSettingsOption option)
        {
            this.option = option;

            text.text = option.Name;
            toggle.isOn = option.GetValue();
        }

        public void OnValueChanged(bool value)
        {
            option.SetValue(value);
        }

        public override void Refresh()
        {
            base.Refresh();

            if (option != null)
            {
                toggle.isOn = option.GetValue();
            }
        }
    }
}
