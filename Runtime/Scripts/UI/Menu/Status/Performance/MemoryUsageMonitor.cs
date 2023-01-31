using Undebugger.UI.Elements;
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

            totalInfo.text = 
                $"{UIUtility.ConvertBytesSizeToReadableString(monitor.AllocatedMemory)} / {UIUtility.ConvertBytesSizeToReadableString(monitor.ReservedMemory)}";
            monoInfo.text = 
                $"{UIUtility.ConvertBytesSizeToReadableString(monitor.MonoUsageMemory)} / {UIUtility.ConvertBytesSizeToReadableString(monitor.MonoHeapSize)}";
            totalUsageBar.Value = monitor.AllocatedMemory / (float)monitor.ReservedMemory;
            monoUsageBar.Value = monitor.MonoUsageMemory / (float)monitor.MonoHeapSize;
        }
    }
}
