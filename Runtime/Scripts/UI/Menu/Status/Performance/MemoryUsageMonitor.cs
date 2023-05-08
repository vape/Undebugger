using System;
using System.Runtime;
using Undebugger.UI.Elements;
using UnityEngine;

namespace Undebugger.UI.Menu.Status.Performance
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class MemoryUsageMonitor : MonoBehaviour
    {
        const string Separator = " / ";

        [SerializeField]
        private FastText totalInfo;
        [SerializeField]
        private FastText monoInfo;
        [SerializeField]
        private HorizontalBar totalUsageBar;
        [SerializeField]
        private HorizontalBar monoUsageBar;

        private void OnEnable()
        {
            totalInfo.SetTextGetter(GetTotalMemoryUsageString);
            monoInfo.SetTextGetter(GetMonoMemoryUsageString);
        }

        private void OnDisable()
        {
            totalInfo.SetTextGetter(null);
            monoInfo.SetTextGetter(null);
        }

        private void Update()
        {
            var monitor = Services.Performance.PerformanceMonitorService.Instance;

            totalInfo.UpdateText();
            monoInfo.UpdateText();

            totalUsageBar.Value = monitor.AllocatedMemory / (float)monitor.ReservedMemory;
            monoUsageBar.Value = monitor.MonoUsageMemory / (float)monitor.MonoHeapSize;
        }

        public void RunGCCollect()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();
        }

        private char[] GetTotalMemoryUsageString(out int start, out int length)
        {
            var monitor = Services.Performance.PerformanceMonitorService.Instance;
            var buffer = FormatUtility.TempBuffer;

            start = 0;
            length = 0;

            FormatUtility.BytesToReadableString(monitor.AllocatedMemory, buffer, ref length);
            FormatUtility.Copy(Separator, buffer, ref length);
            FormatUtility.BytesToReadableString(monitor.ReservedMemory, buffer, ref length);

            return buffer;
        }

        private char[] GetMonoMemoryUsageString(out int start, out int length)
        {
            var monitor = Services.Performance.PerformanceMonitorService.Instance;
            var buffer = FormatUtility.TempBuffer;

            start = 0;
            length = 0;

            FormatUtility.BytesToReadableString(monitor.MonoUsageMemory, buffer, ref length);
            FormatUtility.Copy(Separator, buffer, ref length);
            FormatUtility.BytesToReadableString(monitor.MonoHeapSize, buffer, ref length);

            return buffer;
        }
    }
}
