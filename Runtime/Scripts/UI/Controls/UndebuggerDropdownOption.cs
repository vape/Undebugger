using System;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Controls
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class UndebuggerDropdownOption : MonoBehaviour
    {
        public bool Selected
        {
            get
            {
                return toggle.IsOn;
            }
            set
            {
                toggle.IsOn = value;
            }
        }

        public event Action<int> Clicked;

        [SerializeField]
        private Text label;
        [SerializeField]
        private UndebuggerToggle toggle;

        private int index;

        public void Setup(string text, int index)
        {
            this.index = index;
            label.text = text;
        }

        public void OnClick()
        {
            Clicked?.Invoke(index);
        }
    }
}
