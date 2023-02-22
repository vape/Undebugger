using Undebugger.Model.Commands;
using Undebugger.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Commands
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class TabButton : MonoBehaviour
    {
        public delegate void ClickedDelegate(PageModel model);

        public event ClickedDelegate Clicked;

        public bool Selected
        {
            get
            {
                return toggle.IsOn;
            }
            set
            {
                toggle.IsOn = value;
            }
        }

        [SerializeField]
        private Text nameText;
        [SerializeField]
        private UndebuggerToggle toggle;

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
