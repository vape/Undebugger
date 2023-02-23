using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Settings
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class ButtonSettingsOptionView : SettingsOptionView
    {
        [SerializeField]
        private Text text;

        private ButtonSettingsOption option;

        public void Setup(ButtonSettingsOption option)
        {
            this.option = option;

            text.text = option.Name;
        }

        public void OnClick()
        {
            option.Action?.Invoke();
        }
    }
}
