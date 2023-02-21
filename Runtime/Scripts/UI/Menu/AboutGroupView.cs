using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class AboutGroupView : GroupView
    {
        public override string GroupName => "About";

        [SerializeField]
        private Text versionString;

        private void Awake()
        {
            versionString.text = UndebuggerRoot.Version;
        }
    }
}
