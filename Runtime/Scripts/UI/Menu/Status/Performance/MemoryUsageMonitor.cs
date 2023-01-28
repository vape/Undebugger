using Undebugger.UI.Utility;
using Undebugger.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Status.Performance
{
    internal class MemoryUsageMonitor : MonoBehaviour
    {
        public const float UpdateFrequency = 0.5f;

        [SerializeField]
        private Text totalInfo;
        [SerializeField]
        private Text monoInfo;
        [SerializeField]
        private HorizontalBar totalUsageBar;
        [SerializeField]
        private HorizontalBar monoUsageBar;

        private float nextUpdate;

        private void Update()
        {
            if (Time.realtimeSinceStartup > nextUpdate)
            {
                Refresh();
                nextUpdate = Time.realtimeSinceStartup + UpdateFrequency;
            }
        }

        private void Refresh()
        {
            var monitor = Services.Performance.PerformanceMonitorService.Instance;

            totalInfo.text = $"{Formatter.Format(monitor.AllocatedMemory)} / {Formatter.Format(monitor.ReservedMemory)}";
            monoInfo.text = $"{Formatter.Format(monitor.MonoUsageMemory)} / {Formatter.Format(monitor.MonoHeapSize)}";
            totalUsageBar.Value = monitor.AllocatedMemory / (float)monitor.ReservedMemory;
            monoUsageBar.Value = monitor.MonoUsageMemory / (float)monitor.MonoHeapSize;
        }
    }
}
