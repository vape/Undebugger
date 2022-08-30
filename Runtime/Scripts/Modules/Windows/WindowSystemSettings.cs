using System;
using UnityEngine;

namespace Deszz.Undebugger.UI.Windows
{
    [Serializable]
    public class WindowSystemSettings
    {
        [Serializable]
        private struct IconData
        {
            public string Name;
            public Sprite Sprite;
        }

        public Window WindowTemplate => window;
        public WindowControlButton ControlButtonTemplate => controlButton;

        [SerializeField]
        private Window window;
        [SerializeField]
        private WindowControlButton controlButton;
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
}
