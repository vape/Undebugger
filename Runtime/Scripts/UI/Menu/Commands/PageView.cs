using Deszz.Undebugger.Model.Commands;
using Deszz.Undebugger.UI.Layout;
using UnityEngine;

namespace Deszz.Undebugger.UI.Menu.Commands
{
    public class PageView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform container;
        [SerializeField]
        private SegmentView segmentTemplate;
        [SerializeField]
        private VerticalListLayout layout;

        private SegmentView[] segments;

        public void Init(PageModel model, CommandViewFactory optionViewFactory)
        {
            Deinit();

            segments = new SegmentView[model.Segments.Count];

            for (int i = 0; i < segments.Length; i++)
            {
                segments[i] = Instantiate(segmentTemplate, container);
                segments[i].Init(model.Segments[i], optionViewFactory);
            }
        }

        public void Layout()
        {
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].Layout();
            }

            layout.Layout();
        }

        private void Deinit()
        {
            if (segments != null)
            {
                for (int i = 0; i < segments.Length; ++i)
                {
                    Destroy(segments[i].gameObject);
                }

                segments = null;
            }
        }
    }
}
