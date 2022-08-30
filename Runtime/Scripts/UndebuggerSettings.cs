using Deszz.Undebugger.UI.Menu;
using Deszz.Undebugger.UI.Menu.Commands;
using Deszz.Undebugger.UI.Windows;
using System;
using UnityEngine;

namespace Deszz.Undebugger
{
    // [CreateAssetMenu(fileName = SettingsName, menuName = "Deszz/Undebugger/Internal/Settings")]
    internal class UndebuggerSettings : ScriptableObject
    {
        public const string SettingsName = "Internal_UndebuggerGlobalSettings";

        public static UndebuggerSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<UndebuggerSettings>(UndebuggerSettings.SettingsName);
                }

                return instance;
            }
        }

        private static UndebuggerSettings instance;

        public WindowSystemSettings WindowSettings;
        public MenuView MenuTemplate;
        public CommandView[] CommandTemplates;
        public GroupView[] GroupTemplates;
    }
}
