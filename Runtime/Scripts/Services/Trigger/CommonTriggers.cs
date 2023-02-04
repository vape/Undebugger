namespace Undebugger.Services.Trigger
{
    public static class CommonTriggers
    {
        private static int lastTouchesCount;

        public static MenuTriggerAction CloseMenuWithEscapeKey(bool isOpen)
        {
#if ENABLE_INPUT_SYSTEM
            return
                isOpen && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasReleasedThisFrame ?
                MenuTriggerAction.Close :
                MenuTriggerAction.None;
#else
            return isOpen && UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.Escape) ? MenuTriggerAction.Close : MenuTriggerAction.None;
#endif
        }

        public static MenuTriggerAction ToggleMenuWithF1Key(bool isOpen)
        {
#if ENABLE_INPUT_SYSTEM
            return
                UnityEngine.InputSystem.Keyboard.current.f1Key.wasReleasedThisFrame ?
                    isOpen ?
                    MenuTriggerAction.Close :
                    MenuTriggerAction.Open :
                MenuTriggerAction.None;
#else
            return UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.F1) ? (isOpen ? MenuTriggerAction.Close : MenuTriggerAction.Open) : MenuTriggerAction.None;
#endif
        }

        public static MenuTriggerAction ToggleMenuWithFourFingersTap(bool isOpen)
        {
            var touchesCount = 0;

#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Touchscreen.current == null)
            {
                return MenuTriggerAction.None;
            }

            for (int i = 0; i < UnityEngine.InputSystem.Touchscreen.current.touches.Count; ++i)
            {
                if (UnityEngine.InputSystem.Touchscreen.current.touches[i].isInProgress)
                {
                    touchesCount++;
                }
            }
#else
            touchesCount = UnityEngine.Input.touchCount;
#endif

            var touchesCountChanged = lastTouchesCount != touchesCount;
            lastTouchesCount = touchesCount;

            return
                touchesCountChanged && touchesCount == 4 ?
                    isOpen ?
                    MenuTriggerAction.Close :
                    MenuTriggerAction.Open :
                MenuTriggerAction.None;
        }
    }
}