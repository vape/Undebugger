using Undebugger.Model.Commands;
using Undebugger.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Commands
{
    internal class TabButton : MonoBehaviour
    {
        public delegate void ClickedDelegate(PageModel model);

        public event ClickedDelegate Clicked;

        public bool Selected
        {
            get
            {
                return toggleButton.Selected;
            }
            set
            {
                toggleButton.Selected = value;
            }
        }

        [SerializeField]
        private Text nameText;
        [SerializeField]
        private ToggleButton toggleButton;

        private PageModel model;

        private void OnDestroy()
        {
            Clicked = null;
        }

        public void Init(PageModel model)
        {
            this.model = model;

            nameText.text = model.Name == null ? "Unnamed" : model.Name;
        }

        public void Click()
        {
            if (model != null)
            {
                Clicked?.Invoke(model);
            }
        }
    }
}
