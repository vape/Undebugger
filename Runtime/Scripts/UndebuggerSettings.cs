using Undebugger.UI.Menu;
using Undebugger.UI.Menu.Commands;
using UnityEngine;

namespace Undebugger
{
    // [CreateAssetMenu(fileName = SettingsName, menuName = "Undebugger/Internal/Settings")]
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

        public MenuView MenuTemplate;
        public CommandView[] CommandTemplates;
        public GroupView[] GroupTemplates;
    }
}
