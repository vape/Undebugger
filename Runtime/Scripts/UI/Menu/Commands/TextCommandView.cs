using Undebugger.Model.Commands.Builtin;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Commands
{
    [CommandView(typeof(BaseTextCommandModel))]
    public class TextCommandView : CommandView<BaseTextCommandModel>
    {
        [SerializeField]
        private Text title;
        [SerializeField]
        private InputField inputField;
        [SerializeField]
        private RectTransform content;

        protected override void Setup(BaseTextCommandModel model)
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

            inputField.text = model.DefaultStringValue ?? string.Empty;
            inputField.contentType = model.ContentType;
        }

        public void OnClick()
        {
            model.Apply(inputField.text);
            OnAfterAction();
        }
    }
}
