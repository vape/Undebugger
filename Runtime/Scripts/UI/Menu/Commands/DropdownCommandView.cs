using System.Linq;
using Undebugger.Model.Commands.Builtin;
using Undebugger.UI.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Commands
{
    [CommandView(typeof(DropdownCommandModel))]
    public class DropdownCommandView : CommandView<DropdownCommandModel>
    {
        [SerializeField]
        private Text title;
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private UndebuggerDropdown dropdown;

        protected override void Setup(DropdownCommandModel model)
        {
            base.Setup(model);

            if (model.Title.Value == null)
            {
                title.gameObject.SetActive(false);
                content.Expand();
            }
            else
            {
                title.gameObject.SetActive(true);
                content.offsetMax = new Vector2(0, -title.rectTransform.sizeDelta.y);
                title.text = model.Title;
            }

            dropdown.SetOptions(model.Values.Select(v => v.ToString()).ToArray());
            RefreshValue();
        }

        private void RefreshValue()
        {
            dropdown.SetSelected(model.Index);
        }

        public void OnChanged(int index)
        {
            model.Set(index);
            RefreshValue();
            OnAfterAction();
        }
    }
}
