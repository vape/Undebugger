using Undebugger.Model.Commands.Builtin;
using Undebugger.UI.Controls;
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
        private UndebuggerToggle toggle;

        protected override void Setup(ToggleCommandModel model)
        {
            base.Setup(model);

            toggle.IsOn = model.Reference.Value;
            valueText.text = model.Name;
        }

        public void Toggle(bool value)
        {
            model.Reference.Set(value);
            toggle.IsOn = model.Reference.Value;
            OnAfterAction();
        }
    }
}
