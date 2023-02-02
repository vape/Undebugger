#if (UNITY_EDITOR || DEBUG || UNDEBUGGER) && !UNDEBUGGER_DISABLE
#define UNDEBUGGER_ENABLED
#endif

using Undebugger.Scripts.Services.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Undebugger.Model.Builder;
using UnityEngine;

namespace Undebugger.Services.Trigger
{
    [Flags]
    public enum MenuTriggerAction
    {
        None = 0,
        Close = 1,
        Open = 2
    }

    public delegate MenuTriggerAction MenuTriggerDelegate(bool isOpen);

#if UNDEBUGGER_ENABLED

    public class MenuTriggerService : MonoBehaviour
    {
        public static MenuTriggerService Instance
        {
            get
            {
                return instance;
            }
        }

        private static MenuTriggerService instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (instance == null)
            {
                instance = UndebuggerRoot.CreateServiceInstance<MenuTriggerService>("Menu Trigger Service");
            }
        }

        private List<MenuTriggerDelegate> triggers = new List<MenuTriggerDelegate>(capacity: 3);
        private static MenuModelBuilder builder;

        private void Awake()
        {
            builder = new MenuModelBuilder();
            builder.PreloadAsync();

            RegisterTrigger(CommonTriggers.CloseMenuWithEscapeKey);
            RegisterTrigger(CommonTriggers.ToggleMenuWithFourFingersTap);
            RegisterTrigger(CommonTriggers.ToggleMenuWithF1Key);
        }

        private void Update()
        {
            var isOpen = UIService.Instance.IsMenuOpen;
            var triggerAction = MenuTriggerAction.None;

            for (int i = 0; i < triggers.Count; i++)
            {
                triggerAction |= triggers[i](isOpen);
            }

            if ((triggerAction & MenuTriggerAction.Close) != 0 && isOpen)
            {
                UIService.Instance.CloseMenu();
            }
            else if ((triggerAction & MenuTriggerAction.Open) != 0 && !isOpen)
            {
                UIService.Instance.OpenMenu(builder.Build());
            }
        }

        public void ClearTriggers()
        {
            triggers.Clear();
        }

        public void RegisterTrigger(MenuTriggerDelegate trigger)
        {
            if (trigger == null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            triggers.Add(trigger);
        }
    }
#else
    public class MenuTriggerSerivce
    {
        public static readonly MenuTriggerSerivce Instance = new MenuTriggerSerivce();

        [Conditional("UNDEBUGGER")]
        public void RegisterTrigger(MenuTriggerDelegate trigger)
        { }

        [Conditional("UNDEBUGGER")]
        public void ClearTriggers()
        { }
    }
#endif
}