using Deszz.Undebugger.UI.Menu;
using Deszz.Undebugger.UI.Menu.Commands;
using Deszz.Undebugger.UI.Windows;
using System;
using UnityEngine;

namespace Deszz.Undebugger
{
    [Serializable]
    internal class UndebuggerIcons
    {
        [Serializable]
        private struct IconData
        {
            public string Name;
            public Sprite Sprite;
        }

        [SerializeField]
        private IconData[] icons;

        public Sprite GetIcon(string name)
        {
            for (int i = 0; i < icons.Length; i++)
            {
                if (icons[i].Name == name)
                {
                    return icons[i].Sprite;
                }
            }

            return null;
        }
    }

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

        public Window WindowTemplate;
        public MenuView MenuTemplate;
        public CommandView[] CommandTemplates;
        public GroupView[] GroupTemplates;
        public UndebuggerIcons Icons;
    }
}
