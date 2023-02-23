using Undebugger.Model.Commands.Builtin;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Commands
{
    [CommandView(typeof(ActionCommandModel))]
    public class ActionCommandView : CommandView<ActionCommandModel>
    {
        [SerializeField]
        private Text nameText;

        protected override void Setup(ActionCommandModel model)
        {
            base.Setup(model);

            nameText.text = model.Name;
        }

        public void OnClick()
        {
            model.Activate();
            OnAfterAction();
        }
    }
}
