using Deszz.Undebugger.Model.Commands.Builtin;
using UnityEngine;
using UnityEngine.UI;

namespace Deszz.Undebugger.UI.Menu.Commands
{
    [CommandView(typeof(ActionCommandModel))]
    public class ActionCommandView : CommandView<ActionCommandModel>
    {
        [SerializeField]
        private Text nameText;

        protected override void OnSetup(ActionCommandModel model)
        {
            base.OnSetup(model);

            nameText.text = model.Name;
        }

        public void OnClick()
        {
            model.Activate();
        }
    }
}
