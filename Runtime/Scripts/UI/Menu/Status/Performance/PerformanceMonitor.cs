using UnityEngine;
using UnityEngine.Profiling;
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
        [SerializeField]
        private Text memoryUsage;

        private float nextStatsUpdate;
        private int previousFps;

        private void Awake()
        {
            averageFps.text = "...";
            memoryUsage.text = "...";
        }

        private void Update()
        {
            var fps = (int)graph.TargetFPSHint;
            if (fps != previousFps)
            {
                UpdateGraphHints();
                previousFps = fps;
            }

            if (Time.realtimeSinceStartup > nextStatsUpdate)
            {
                var monitor = UndebuggerPerformanceMonitor.Instance;
                if (monitor != null)
                {
                    UpdateQuickStats(monitor);
                    nextStatsUpdate = Time.realtimeSinceStartup + QuickStatsUpdateFrequency;
                }
            }
        }

        private void UpdateQuickStats(UndebuggerPerformanceMonitor monitor)
        {
            const int div = 1024 * 1024;

            var allocated = Profiler.GetTotalAllocatedMemoryLong() / div;
            var reserved = Profiler.GetTotalReservedMemoryLong() / div;

            averageFps.text = $"Average FPS: {1f / monitor.MeanFrametime:0} ({monitor.MeanFrametime * 1000:0.0}ms)";
            memoryUsage.text = $"Memory (allocated/reserved): {allocated} MB / {reserved} MB";
        }

        private void UpdateGraphHints()
        {
            graphMidHint.text = $"FPS:{graph.TargetFPSHint:0}({(1f / graph.TargetFPSHint) * 1000:0.0}ms)";
            graphTopHint.text = $"FPS:{graph.MinFPSHint:0}({(1f / graph.MinFPSHint) * 1000:0.0}ms)";
        }
    }
}
