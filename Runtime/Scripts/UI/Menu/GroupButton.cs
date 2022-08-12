using TMPro;
using UnityEngine;

namespace Deszz.Undebugger.UI.Menu
{
    internal class GroupButton : MonoBehaviour
    {
        public delegate void ClickedDelegate(int index);

        public event ClickedDelegate Clicked;

        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private GameObject selectionHighlight;

        private int index;

        private void OnDestroy()
        {
            Clicked = null;
        }

        public void SetSelected(bool selected)
        {
            selectionHighlight.SetActive(selected);
            nameText.fontStyle = selected ? (nameText.fontStyle | FontStyles.Bold) : (nameText.fontStyle & ~FontStyles.Bold);
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
