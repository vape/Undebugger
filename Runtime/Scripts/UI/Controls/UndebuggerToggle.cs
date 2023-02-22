using System;
using UnityEngine;
using UnityEngine.Events;

namespace Undebugger.UI.Controls
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class UndebuggerToggle : UndebuggerClickable
    {
        public bool IsOn
        {
            get
            {
                return isOn;
            }
            set
            {
                isOn = value;
                checkmark.gameObject.SetActive(isOn);
            }
        }

        private bool isOn;

        [SerializeField]
        private GameObject checkmark;
        [SerializeField]
        private UnityEvent<bool> onChanged;

        private void OnEnable()
        {
            IsOn = isOn;
        }

        protected override void OnClick()
        {
            onChanged?.Invoke(!IsOn);
        }
    }
}
