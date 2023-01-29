using System.Collections.Generic;
using Undebugger.Model.Commands;
using Undebugger.UI.Layout;
using UnityEngine;

namespace Undebugger.UI.Menu.Commands
{
    internal class PageView : MonoBehaviour, IPoolable, IPoolHandler
    {
        [SerializeField]
        private RectTransform container;
        [SerializeField]
        private SegmentView segmentTemplate;
        [SerializeField]
        private VerticalListLayout layout;

        private List<SegmentView> segments;
        private MenuPool pool;

        public void UsePool(MenuPool pool)
        {
            this.pool = pool;
        }

        public void AddingToPool()
        {
            Deinit();
        }

        public void Init( PageModel model, CommandViewFactory optionViewFactory)
        {
            Deinit();

            if (segments == null || model.Segments.Count / segments.Capacity >= 2)
            {
                segments = new List<SegmentView>(model.Segments.Count);
            }

            for (int i = 0; i < model.Segments.Count; i++)
            {
                var segment = pool.GetOrInstantiate(segmentTemplate, container);
                segment.Init(model.Segments[i], optionViewFactory);

                segments.Add(segment);
            }

            LayoutUtility.SetLayoutDirty(transform, LayoutDirtyFlag.All);
        }

        private void Deinit()
        {
            if (segments != null)
            {
                for (int i = 0; i < segments.Count; ++i)
                {
                    if (segments[i] == null)
                    {
                        continue;
                    }

                    pool.Add(segments[i]);
                }

                segments.Clear();
            }
        }
    }
}
