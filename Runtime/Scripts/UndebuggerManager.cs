using Undebugger.Builder;
using Undebugger.UI.Menu;
using System;
using System.Collections.Generic;
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
        private static MenuTriggerAction CloseByEscape(bool isOpen)
        {
            return isOpen && Input.GetKeyUp(KeyCode.Escape) ? MenuTriggerAction.Close : MenuTriggerAction.None;
        }

        private static MenuTriggerAction ToggleByF1(bool isOpen)
        {
            return Input.GetKeyUp(KeyCode.F1) ? (isOpen ? MenuTriggerAction.Close : MenuTriggerAction.Open) : MenuTriggerAction.None;
        }

        private static MenuTriggerAction ToggleByFourFingers(bool isOpen)
        {
            return Input.touchCount == 4 ? (isOpen ? MenuTriggerAction.Close : MenuTriggerAction.Open) : MenuTriggerAction.None;
        }

        public static UndebuggerManager Instance
        { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        private static void Create()
        {
            var gameObject = new GameObject("Undebugger");
            gameObject.hideFlags = HideFlags.NotEditable;
            DontDestroyOnLoad(gameObject);

            Instance = gameObject.AddComponent<UndebuggerManager>();
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
            var model = ModelBuilder.Build();
            var settings = UndebuggerSettings.Instance;

            using (sceneManager.MakeActive())
            {
                var menu = GameObject.Instantiate(settings.MenuTemplate, sceneManager.GetSafeArea().Rect);
                menu.name = settings.MenuTemplate.name;
                menu.Load(model, new MenuContext() { Settings = settings });

                return menu;
            }
        }
    }
}
