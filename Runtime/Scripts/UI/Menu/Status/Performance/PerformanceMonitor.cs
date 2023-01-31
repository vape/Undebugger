using Undebugger.Services.Performance;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Status.Performance
{
    public class PerformanceMonitor : MonoBehaviour
    {
        public const float QuickStatsUpdateFrequency = 1f;

        [SerializeField]
        private FrametimeGraph graph;
        [SerializeField]
        private Text graphMidHint;
        [SerializeField]
        private Text graphTopHint;
        [SerializeField]
        private Text averageFps;

        private float nextStatsUpdate;
        private int previousFps;

        private void Awake()
        {
            averageFps.text = "...";
        }

        private void Update()
        {
            var fps = (int)(1f / graph.GetFrametimeAtStep(1));
            if (fps != previousFps)
            {
                UpdateGraphHints();
                previousFps = fps;
            }

            if (Time.realtimeSinceStartup > nextStatsUpdate)
            {
                var monitor = PerformanceMonitorService.Instance;
                if (monitor != null)
                {
                    UpdateQuickStats(monitor);
                    nextStatsUpdate = Time.realtimeSinceStartup + QuickStatsUpdateFrequency;
                }
            }
        }

        private void UpdateQuickStats(PerformanceMonitorService monitor)
        {
            averageFps.text = $"Average FPS: {1f / monitor.MeanFrameTime:0} ({monitor.MeanFrameTime * 1000:0.0}ms)";
        }

        private void UpdateGraphHints()
        {
            var mid = graph.GetFrametimeAtStep(1);
            var top = graph.GetFrametimeAtStep(2);

            graphMidHint.text = $"FPS:{1f / mid:0}({(mid) * 1000:0.0}ms)";
            graphTopHint.text = $"FPS:{1f / top:0}({(top) * 1000:0.0}ms)";
        }
    }
}
