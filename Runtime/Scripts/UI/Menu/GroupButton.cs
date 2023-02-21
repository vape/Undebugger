using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class GroupButton : MonoBehaviour
    {
        public delegate void ClickedDelegate(int index);

        public event ClickedDelegate Clicked;

        [SerializeField]
        private Text nameText;
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
            nameText.fontStyle = selected ? FontStyle.Bold : FontStyle.Normal;
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
