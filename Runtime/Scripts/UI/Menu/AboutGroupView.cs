using TMPro;
using UnityEngine;

namespace Deszz.Undebugger.UI.Menu
{
    internal class AboutGroupView : GroupView
    {
        private class Manifest
        {
#pragma warning disable IDE1006 // Naming Styles
            public string version;
#pragma warning restore IDE1006 // Naming Styles
        }

        public override string GroupName => "About";

        [SerializeField]
        private TextAsset packageManifest;
        [SerializeField]
        private TMP_Text versionString;

        private void Awake()
        {
            versionString.text = JsonUtility.FromJson<Manifest>(packageManifest.text).version;
        }
    }
}
