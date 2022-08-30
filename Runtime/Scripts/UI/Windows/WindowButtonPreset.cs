using System;
using UnityEngine;

namespace Deszz.Undebugger.UI.Windows
{
    [Serializable]
    public struct WindowButtonPreset
    {
        public Sprite Icon;
        public Action Action;
    }
}
