using Deszz.Undebugger.Model.Commands;
using Deszz.Undebugger.UI.Layout;
using UnityEngine;

namespace Deszz.Undebugger.UI.Menu.Commands
{
    public class SegmentView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform container;
        [SerializeField]
        private FlexibleGrid layout;

        private CommandView[] commands;

        public void Init(SegmentModel model, CommandViewFactory optionViewFactory)
        {
            Deinit();

            commands = new CommandView[model.Commands.Count];

            for (int i = 0; i < model.Commands.Count; ++i)
            {
                var template = optionViewFactory.FindTemplate(model.Commands[i].GetType());

                commands[i] = Instantiate(template, container);
                commands[i].Setup(model.Commands[i]);
            }
        }

        public void Layout()
        {
            layout.Layout();
        }

        private void Deinit()
        {
            if (commands != null)
            {
                for (int i = 0; i < commands.Length; ++i)
                {
                    Destroy(commands[i].gameObject);
                }

                commands = null;
            }
        }
    }
}
