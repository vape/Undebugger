using Undebugger.Model;
using UnityEngine;

namespace Undebugger.UI.Menu.Status
{
    public class StatusGroupView : GroupView
    {
        public override string GroupName => "Status";

        [SerializeField]
        private StatusSegmentView segmentTemplate;
        [SerializeField]
        private Transform container;

        private StatusSegmentView[] segments;

        public override void Load(MenuModel menuModel, MenuContext menuContext)
        {
            base.Load(menuModel, menuContext);

            segments = new StatusSegmentView[menuModel.Status.Segments.Count];

            for (int i = 0; i < segments.Length; i++)
            {
                segments[i] = Instantiate(segmentTemplate, container);
                segments[i].Init(menuModel.Status.Segments[i]);
            }
        }
    }
}
