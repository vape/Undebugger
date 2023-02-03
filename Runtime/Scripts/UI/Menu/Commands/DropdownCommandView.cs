using System.Linq;
using Undebugger.Model.Commands.Builtin;
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
        private Dropdown dropdown;
        [SerializeField]
        private RectTransform content;

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

            dropdown.options = model.Values.Select(v => new Dropdown.OptionData(v.ToString())).ToList();
            RefreshValue();
        }

        private void RefreshValue()
        {
            dropdown.value = model.Index;
        }

        public void OnChanged(int index)
        {
            model.Set(index);
            RefreshValue();
        }
    }
}
