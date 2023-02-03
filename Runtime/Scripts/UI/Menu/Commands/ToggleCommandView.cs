using Undebugger.Model.Commands.Builtin;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Commands
{
    [CommandView(typeof(ToggleCommandModel))]
    public class ToggleCommandView : CommandView<ToggleCommandModel>
    {
        [SerializeField]
        private Text valueText;
        [SerializeField]
        public Toggle toggle;

        protected override void Setup(ToggleCommandModel model)
        {
            base.Setup(model);

            toggle.SetIsOnWithoutNotify(model.Reference.Value);
            valueText.text = model.Name;
        }

        public void Toggle(bool value)
        {
            model.Reference.Set(value);
            toggle.SetIsOnWithoutNotify(model.Reference.Value);
        }
    }
}
