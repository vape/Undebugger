using Deszz.Undebugger.Model.Commands.Builtin;
using TMPro;
using UnityEngine;

namespace Deszz.Undebugger.UI.Menu.Commands
{
    [CommandView(typeof(ActionCommandModel))]
    public class ActionCommandView : CommandView<ActionCommandModel>
    {
        [SerializeField]
        private TMP_Text nameText;

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
