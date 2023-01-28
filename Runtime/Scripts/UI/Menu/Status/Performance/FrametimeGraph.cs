using Undebugger.Services.Performance;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Status.Performance
{
    public class FrametimeGraph : Graphic
    {
        public float MinFPSHint
        { get; private set; } = 30;
        public float TargetFPSHint
        { get; private set; } = 60;

        [SerializeField]
        private int minBarWidth = 3;
        [SerializeField]
        private int minSpaceBetweenBars = 1;
        [SerializeField]
        private int frameWidth = 1;
        [SerializeField]
        private int baseLineWidth = 1;
        [SerializeField]
        private Color frameColor = Color.white;
        [SerializeField]
        private Color backgroundColor = Color.gray;
        [SerializeField]
        private Color goodFrameTimeColor = Color.green;
        [SerializeField]
        private Color badFrameTimeColor = Color.red;
        [SerializeField]
        [Range(0.5f, 3f)]
        private float frameTimeColorSensitivity = 1;

        private UIVertex[] quad = new UIVertex[4];

        private void Update()
        {
            const float snap = 30;

            var monitor = PerformanceMonitorService.Instance;
            if (monitor == null)
            {
                return;
            }

            var fpsTarget = 1f / monitor.TargetFrameTime;

            TargetFPSHint = Mathf.Max(1, Mathf.Round(fpsTarget / snap)) * snap;
            MinFPSHint = TargetFPSHint / 2f;

            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var rect = GetPixelAdjustedRect();

            PopulateBackground(ref rect, vh);

            if (PerformanceMonitorService.Instance != null
#if UNITY_EDITOR
                || !Application.isPlaying
#endif
                )
            {
                PopulateGraph(ref rect, vh);
            }
            
            PopulateFrame(ref rect, vh);
        }

        private void PopulateBackground(ref Rect rect, VertexHelper vh)
        {
            FillQuad(ref quad, rect.x, rect.y, rect.x + rect.width, rect.y + rect.height, backgroundColor);
            vh.AddUIVertexQuad(quad);
        }

        private void PopulateFrame(ref Rect rect, VertexHelper vh)
        {
            if (frameWidth > 0)
            {
                FillQuad(ref quad,
                rect.x,
                rect.y,
                rect.x + frameWidth,
                rect.y + rect.height,
                frameColor);
                vh.AddUIVertexQuad(quad);

                FillQuad(ref quad,
                    rect.x + rect.width - frameWidth,
                    rect.y,
                    rect.x + rect.width,
                    rect.y + rect.height,
                    frameColor);
                vh.AddUIVertexQuad(quad);

                FillQuad(ref quad,
                    rect.x + frameWidth,
                    rect.y + rect.height - frameWidth,
                    rect.x + rect.width - frameWidth,
                    rect.y + rect.height,
                    frameColor);
                vh.AddUIVertexQuad(quad);

                FillQuad(ref quad,
                    rect.x + frameWidth,
                    rect.y,
                    rect.x + rect.width - frameWidth,
                    rect.y + frameWidth,
                    frameColor);
                vh.AddUIVertexQuad(quad);
            }
            
            if (baseLineWidth > 0)
            {
                FillQuad(ref quad,
                rect.x + frameWidth,
                rect.y + rect.height / 2 - Mathf.Max(1, baseLineWidth / 2),
                rect.x + rect.width - frameWidth,
                rect.y + rect.height / 2 + baseLineWidth / 2,
                frameColor);
                vh.AddUIVertexQuad(quad);
            }
        }

        private void PopulateGraph(ref Rect rect, VertexHelper vh)
        {
            var preferedBarsCount = (int)(rect.width / (minBarWidth + minSpaceBetweenBars));
            var bars = Mathf.Min(PerformanceMonitorService.FrameBufferSize, preferedBarsCount);
            var scale = preferedBarsCount / (float)bars;
            var offset = new Vector2(-rect.width * rectTransform.pivot.x, -rect.height * rectTransform.pivot.y);

            var spaceBetweenBars = minSpaceBetweenBars * scale;
            var barWidth = minBarWidth * scale;
            var barHeight = rect.height / 2;
            var midTime = 1f / TargetFPSHint;

            for (int i = 0; i < bars; ++i)
            {
                ref var frame = ref GetFrame(i);

                var color = Color.Lerp(goodFrameTimeColor, badFrameTimeColor, frame.Tier * frameTimeColorSensitivity);
                var value = frame.Time / midTime;

                var x0 = rect.x + i * spaceBetweenBars + i * barWidth;
                var y0 = rect.y;

                FillQuad(ref quad, x0, y0, x0 + barWidth, y0 + value * barHeight, color);
                vh.AddUIVertexQuad(quad);
            }
        }

#if UNITY_EDITOR
        private static Frame stubFrame;
#endif

        private static ref Frame GetFrame(int index)
        {
#if UNITY_EDITOR
            const float Target = 0.016f;

            if (!Application.isPlaying)
            {
                stubFrame.Time = 0.016f + 0.016f * Mathf.Sin(index / 10f);
                stubFrame.Tier = Target > stubFrame.Time ? 0 : (stubFrame.Time - Target) / Target;

                return ref stubFrame;
            }
#endif

            return ref PerformanceMonitorService.Instance.GetFrame(index);
        }

        private static void FillQuad(ref UIVertex[] buffer, float x0, float y0, float x1, float y1, Color color)
        {
            buffer[0].position.x = x0;
            buffer[0].position.y = y0;
            buffer[0].color = color;

            buffer[1].position.x = x1;
            buffer[1].position.y = y0;
            buffer[1].color = color;

            buffer[2].position.x = x1;
            buffer[2].position.y = y1;
            buffer[2].color = color;

            buffer[3].position.x = x0;
            buffer[3].position.y = y1;
            buffer[3].color = color;
        }
    }
}