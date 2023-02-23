using Undebugger.Model.Commands.Builtin;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Commands
{
    [CommandView(typeof(CarouselCommandModel))]
    public class CarouselCommandView : CommandView<CarouselCommandModel>
    {
        [SerializeField]
        private Text title;
        [SerializeField]
        private Text valueText;
        [SerializeField]
        private RectTransform content;

        protected override void Setup(CarouselCommandModel model)
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
            OnAfterAction();
        }

        public void PrevClick()
        {
            model.Set(model.Index - 1);
            RefreshValue();
            OnAfterAction();
        }
    }
}
