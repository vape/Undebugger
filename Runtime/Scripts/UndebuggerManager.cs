using System;
using System.Collections.Generic;
using Undebugger.Builder;
using Undebugger.UI.Menu;
using UnityEngine;

namespace Undebugger
{
    [Flags]
    public enum MenuTriggerAction
    {
        None = 0,
        Close = 1,
        Open = 2
    }

    public delegate MenuTriggerAction MenuTriggerDelegate(bool isOpen);

    public class UndebuggerManager : MonoBehaviour
    {
        public const string VersionString = "1.0.0";

        private const string MenuViewTemplateName = "Undebugger Menu View";

        private static int lastTouchesCount = 0;

        private static MenuTriggerAction CloseByEscape(bool isOpen)
        {
#if ENABLE_INPUT_SYSTEM
            return
                isOpen && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasReleasedThisFrame ?
                MenuTriggerAction.Close :
                MenuTriggerAction.None;
#else
            return isOpen && Input.GetKeyUp(KeyCode.Escape) ? MenuTriggerAction.Close : MenuTriggerAction.None;
#endif
        }

        private static MenuTriggerAction ToggleByF1(bool isOpen)
        {
#if ENABLE_INPUT_SYSTEM
            return
                UnityEngine.InputSystem.Keyboard.current.f1Key.wasReleasedThisFrame ?
                    isOpen ?
                    MenuTriggerAction.Close :
                    MenuTriggerAction.Open :
                MenuTriggerAction.None;
#else
            return Input.GetKeyUp(KeyCode.F1) ? (isOpen ? MenuTriggerAction.Close : MenuTriggerAction.Open) : MenuTriggerAction.None;
#endif
        }

        private static MenuTriggerAction ToggleByFourFingers(bool isOpen)
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
            touchesCount = Input.touchCount;
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

        public static UndebuggerManager Instance
        {
            get
            {
                if (!created)
                {
                    Create();
                }

                return instance;
            }
        }

        private static bool created;
        private static UndebuggerManager instance;

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            if (!created)
            {
                Create();
            }
        }

        private static void Create()
        {
            var gameObject = new GameObject("Undebugger");
            gameObject.hideFlags = HideFlags.NotEditable;
            DontDestroyOnLoad(gameObject);

            instance = gameObject.AddComponent<UndebuggerManager>();
            created = true;
        }

        public bool IsOpen
        { get { return menuView != null; } }
        public List<MenuTriggerDelegate> Triggers
        { get; private set; } = new List<MenuTriggerDelegate>(capacity: 3)
        {
            CloseByEscape,
            ToggleByF1,
            ToggleByFourFingers,
        };

        private MenuView menuView;
        private UndebuggerSceneManager sceneManager;
        private MenuView menuViewTemplate;

        private void Awake()
        {
            sceneManager = new UndebuggerSceneManager();
        }

        private void Update()
        {
            var isOpen = IsOpen;
            var triggerAction = MenuTriggerAction.None;

            for (int i = 0; i < Triggers.Count; i++)
            {
                triggerAction |= Triggers[i](isOpen);
            }

            if (menuView != null && (triggerAction & MenuTriggerAction.Close) != 0)
            {
                TryDestroyMenu();
            }
            else if (menuView == null && (triggerAction & MenuTriggerAction.Open) != 0)
            {
                TryCreateMenu();
            }
        }

        public bool TryCreateMenu()
        {
            if (IsOpen)
            {
                return false;
            }

            menuView = CreateMenu();
            return true;
        }

        public bool TryDestroyMenu()
        {
            if (!IsOpen)
            {
                return false;
            }

            Destroy(menuView.gameObject);
            return true;
        }

        private MenuView CreateMenu()
        {
            if (menuViewTemplate == null)
            {
                menuViewTemplate = Resources.Load<MenuView>(MenuViewTemplateName);
            }

            var model = ModelBuilder.Build();

            using (sceneManager.MakeActive())
            {
                var menu = GameObject.Instantiate(menuViewTemplate, sceneManager.GetSafeArea().Rect);
                menu.name = menuViewTemplate.name;
                menu.Load(model);

                return menu;
            }
        }
    }
}
