using System;
using UnityEngine;
using UnityEngine.UI;

namespace Deszz.Undebugger.UI.Windows
{
    public class WindowHeaderButton : MonoBehaviour
    {
        [SerializeField]
        private Image icon;

        private Action callback;

        public void Setup(Sprite icon, Action callback)
        {
            this.icon.sprite = icon;
            this.callback = callback;
        }

        public void OnClick()
        {
            callback?.Invoke();
        }
    }
}
