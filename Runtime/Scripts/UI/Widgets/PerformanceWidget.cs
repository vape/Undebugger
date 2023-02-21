using System;
using Undebugger.Services.Performance;
using Undebugger.UI.Elements;
using UnityEngine;

namespace Undebugger.UI.Widgets.Performance
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class PerformanceWidget : Widget
    {
        [SerializeField]
        private FastText fpsText;

        private int previousFps;

        private void OnEnable()
        {
            fpsText.SetTextGetter(GetFramerateString);
        }

        private void OnDisable()
        {
            fpsText.SetTextGetter(null);
        }

        private void Update()
        {
            var fps = GetCurrentFramerate();
            if (fps != previousFps)
            {
                fpsText.UpdateText();
                previousFps = fps;
            }
        }

        private char[] GetFramerateString(out int start, out int length)
        {
            var buffer = FormatUtility.TempBuffer;

            start = 0;
            length = 0;

            FormatUtility.WriteInt32(GetCurrentFramerate(), buffer, ref length);

            return buffer;
        }

        private int GetCurrentFramerate()
        {
#if UNITY_EDITOR
            if (PerformanceMonitorService.Instance == null)
            {
                return 1234;
            }
#endif

            return Mathf.RoundToInt(1f / PerformanceMonitorService.Instance.SmoothedFrameTime);
        }
    }
}
