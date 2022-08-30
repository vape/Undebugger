using Deszz.Undebugger.UI;
using Deszz.Undebugger.UI.Layout;
using Deszz.Undebugger.UI.Windows;
using UnityEngine;

namespace Deszz.UndebuggerUI.Menu
{
    public class MenuViewBackground : MonoBehaviour, IWindowContent
    {
        [SerializeField]
        private SafeAreaIgnore safeAreaIgnore;

        private Window window;

        public void AddingToWindow(Window window)
        {
            this.window = window;
            window.ChangingWindowMode += ChangingWindowModeHandler;
        }

        public void RemovingFromWindow()
        {
            window.ChangingWindowMode -= ChangingWindowModeHandler;
            window = null;
        }

        private void ChangingWindowModeHandler(Window window, WindowMode mode)
        {
            switch (mode)
            {
                case WindowMode.Windowed:
                    safeAreaIgnore.enabled = false;
                    safeAreaIgnore.GetComponent<RectTransform>().Expand();
                    break;
                case WindowMode.Maximized:
                    safeAreaIgnore.enabled = true;
                    break;
            }
        }
    }
}
