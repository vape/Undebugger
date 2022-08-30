using Deszz.Undebugger.UI.Layout;
using Deszz.Undebugger.UI.Windows;
using UnityEngine;

namespace Deszz.Undebugger.UI.Menu
{
    public class MenuViewWindowHandler : MonoBehaviour, IWindowContent
    {
        [SerializeField]
        private SafeAreaIgnore backgroundSafeAreaIgnore;
        [SerializeField]
        private LayoutMaster layoutMaster;

        public void AddingToWindow(Window window)
        {
            window.ChangingWindowState += ChangingWindowStateHandler;
            window.ChangedWindowState += ChangedWindowStateHandler;
        }

        public void RemovedFromWindow(Window window)
        {
            window.ChangingWindowState -= ChangingWindowStateHandler;
            window.ChangedWindowState -= ChangedWindowStateHandler;
        }

        private void ChangingWindowStateHandler(Window window, WindowState state)
        {
            switch (state)
            {
                case WindowState.Windowed:
                    backgroundSafeAreaIgnore.enabled = false;
                    backgroundSafeAreaIgnore.GetComponent<RectTransform>().Expand();
                    break;
                case WindowState.Maximized:
                    backgroundSafeAreaIgnore.enabled = true;
                    break;
            }
        }

        private void ChangedWindowStateHandler(Window window, WindowState state)
        {
            layoutMaster.SetDirty();
            layoutMaster.ForceRefresh();
        }
    }
}
