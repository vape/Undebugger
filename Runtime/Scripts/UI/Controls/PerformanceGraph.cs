using System;
using Undebugger.UI.Elements;
using UnityEngine;

namespace Undebugger.UI.Controls
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class PerformanceGraph : MonoBehaviour
    {
        [SerializeField]
        private FrametimeGraph graph;
        [SerializeField]
        private FastText midHint;
        [SerializeField]
        private FastText topHint;
        [SerializeField]
        private bool shortHint;

        private int lastHint;

        private void OnEnable()
        {
            midHint.SetTextGetter(GetMidHintText);
            topHint.SetTextGetter(GetTopHintText);
        }

        private void OnDisable()
        {
            midHint.SetTextGetter(null);
            topHint.SetTextGetter(null);
        }

        private void Update()
        {
            var hint = (int)(1f / graph.GetFrametimeAtStep(1));
            if (hint != lastHint)
            {
                midHint.UpdateText();
                topHint.UpdateText();
                lastHint = hint;
            }
        }

        private char[] GetMidHintText(out int start, out int length)
        {
            return GetFormattedHint(1, out start, out length);
        }

        private char[] GetTopHintText(out int start, out int length)
        {
            return GetFormattedHint(2, out start, out length);
        }

        private char[] GetFormattedHint(int step, out int start, out int length)
        {
            const string prefix = "FPS:";

            var buffer = FormatUtility.TempBuffer;

            start = 0;
            length = 0;

            if (!shortHint)
            {
                FormatUtility.Copy(prefix, buffer, ref length);
            }

            FormatUtility.FrametimeToReadableString(graph.GetFrametimeAtStep(step), shortHint, buffer, ref length);

            return buffer;
        }
    }
}
