using Undebugger.UI.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Widgets.Performance
{
    internal class PerformanceWidget : Widget
    {
        [SerializeField]
        private FrametimeGraph graph;
        [SerializeField]
        private Text graphMidHint;
        [SerializeField]
        private Text graphTopHint;

        private int previousFps;

        private void Update()
        {
            var fps = (int)(1f / graph.GetFrametimeAtStep(1));
            if (fps != previousFps)
            {
                UpdateGraphHints();
                previousFps = fps;
            }
        }

        private void UpdateGraphHints()
        {
            var mid = graph.GetFrametimeAtStep(1);
            var top = graph.GetFrametimeAtStep(2);

            graphMidHint.text = $"{1f / mid:0}({(mid) * 1000:0}ms)";
            graphTopHint.text = $"{1f / top:0}({(top) * 1000:0}ms)";
        }
    }
}
