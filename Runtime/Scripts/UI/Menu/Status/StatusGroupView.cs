using System.Collections.Generic;
using Undebugger.Model;
using UnityEngine;

namespace Undebugger.UI.Menu.Status
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class StatusGroupView : GroupView
    {
        public override string GroupName => "Status";

        [SerializeField]
        private StatusSegmentView segmentTemplate;
        [SerializeField]
        private Transform container;

        private List<StatusSegmentView> segments;

        private void OnDestroy()
        {
            Unload();
        }

        public override void AddingToPool()
        {
            base.AddingToPool();

            Unload();
        }

        public override void Load(MenuModel menuModel)
        {
            base.Load(menuModel);

            Unload();

            if (segments == null)
            {
                segments = new List<StatusSegmentView>(menuModel.Status.Segments.Count);
            }

            for (int i = 0; i < menuModel.Status.Segments.Count; i++)
            {
                var segment = pool.GetOrInstantiate(segmentTemplate, container);
                segment.Init(menuModel.Status.Segments[i]);

                segments.Add(segment);
            }
        }

        public void Unload()
        {
            if (segments != null)
            {
                for (int i = 0; i < segments.Count; ++i)
                {
                    pool.AddOrDestroy(segments[i]);
                }

                segments.Clear();
            }
        }
    }
}
