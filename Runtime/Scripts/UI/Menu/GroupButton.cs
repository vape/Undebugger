using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deszz.Undebugger.UI.Menu
{
    internal class GroupButton : MonoBehaviour
    {
        public delegate void ClickedDelegate(int index);

        public event ClickedDelegate Clicked;

        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private Outline selectionOutline;

        private int index;

        private void OnDestroy()
        {
            Clicked = null;
        }

        public void SetSelected(bool selected)
        {
            selectionOutline.enabled = selected;
        }

        public void Init(int index, string name)
        {
            this.index = index;
            nameText.text = name == null ? "Unnamed" : name;
        }

        public void Click()
        {
            Clicked?.Invoke(index);
        }
    }
}
