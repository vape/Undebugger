using Deszz.Undebugger.Model.Commands.Builtin;
using UnityEngine;
using UnityEngine.UI;

namespace Deszz.Undebugger.UI.Menu.Commands
{
    [CommandView(typeof(CarouselCommandModel))]
    public class CarouselCommandView : CommandView<CarouselCommandModel>
    {
        [SerializeField]
        private Text valueText;

        protected override void OnSetup(CarouselCommandModel model)
        {
            base.OnSetup(model);

            RefreshValue();
        }

        private void RefreshValue()
        {
            valueText.text = model.Value.ToString();
        }

        public void NextClick()
        {
            model.Set(model.Index + 1);
            RefreshValue();
        }

        public void PrevClick()
        {
            model.Set(model.Index - 1);
            RefreshValue();
        }
    }
}
