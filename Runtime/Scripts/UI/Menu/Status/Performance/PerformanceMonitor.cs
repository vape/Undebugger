using Undebugger.Services.Performance;
using Undebugger.UI.Elements;
using UnityEngine;

namespace Undebugger.UI.Menu.Status.Performance
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class PerformanceMonitor : MonoBehaviour
    {
        [SerializeField]
        private FastText meanFpsText;

        private int lastMeanFps;

        private void OnEnable()
        {
            meanFpsText.SetTextGetter(GetMeanFpsString);
        }

        private void OnDisable()
        {
            meanFpsText.SetTextGetter(null);
        }

        private void Update()
        {
            var meanFps = (int)(1f / PerformanceMonitorService.Instance.MeanFrameTime);
            if (meanFps != lastMeanFps)
            {
                meanFpsText.UpdateText();
                lastMeanFps = meanFps;
            }
        }

        private char[] GetMeanFpsString(out int start, out int length)
        {
            const string prefix = "Average FPS: ";

            var buffer = FormatUtility.TempBuffer;

            start = 0;
            length = 0;

            FormatUtility.Copy(prefix, buffer, ref length);
            FormatUtility.FrametimeToReadableString(PerformanceMonitorService.Instance.MeanFrameTime, false, buffer, ref length);

            return buffer;
        }
    }
}
