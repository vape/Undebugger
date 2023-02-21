using Undebugger.Services.Performance;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Elements
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    [RequireComponent(typeof(CanvasRenderer))]
    [ExecuteAlways]
    internal class FrametimeGraph : Graphic
    {
        private static readonly float[] steps = new float[]
        {
            1f / 15,
            1f / 30,
            1f / 60,
            1f / 90,
            1f / 120,
            1f / 240,
            1f / 300
        };

        private static UIVertex[] quad = new UIVertex[4];

#if UNITY_EDITOR
        private static Frame stubFrame;
#endif

        private static ref Frame GetFrame(int index)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                stubFrame.Time = 0.016f + 0.008f * Mathf.Sin(index / 10f);

                return ref stubFrame;
            }
#endif

            return ref PerformanceMonitorService.Instance.GetFrame(index);
        }

        [SerializeField]
        private int minBarWidth = 3;
        [SerializeField]
        private int minSpaceBetweenBars = 1;
        [SerializeField]
        private int frameWidth = 1;
        [SerializeField]
        private int baseLineWidth = 1;
        [SerializeField]
        private Color32 frameColor = Color.white;
        [SerializeField]
        private Color32 backgroundColor = Color.gray;
        [SerializeField]
        private Color32[] colors = new Color32[]
        {
            new Color32(0,   255, 255, 255),
            new Color32(0,   255, 0,   255),
            new Color32(255, 255, 0,   255),
            new Color32(255, 0,   0,   255)
        };

        private float[] colorSteps = new float[4];
        private float targetFrameTime = 1f / 60f;

        private void Update()
        {
            var time = 0.01666f;
            var step = 1;

            var monitor = PerformanceMonitorService.Instance;
            if (monitor != null)
            {
                time = monitor.TargetFrameTime;
                step = 0;

                while (step < steps.Length - 1 && time < steps[step])
                {
                    step++;
                }
            }

            targetFrameTime = Mathf.Round(time / steps[step]) * steps[step];

            colorSteps[0] = targetFrameTime / 2f;
            colorSteps[1] = targetFrameTime;
            colorSteps[2] = targetFrameTime * 1.5f;
            colorSteps[3] = targetFrameTime * 3.0f;

            SetVerticesDirty();
        }

        public float GetFrametimeAtStep(int step)
        {
            return targetFrameTime / 2 * Mathf.Pow(2, step);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var rect = GetPixelAdjustedRect();

            vh.Clear();

            PopulateBackground(ref rect, vh);
            PopulateGraphNew(ref rect, vh);
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
                    rect.y + rect.height / 3 - Mathf.Max(1, baseLineWidth / 2),
                    rect.x + rect.width - frameWidth,
                    rect.y + rect.height / 3 + baseLineWidth / 2,
                    frameColor);
                vh.AddUIVertexQuad(quad);

                FillQuad(ref quad,
                    rect.x + frameWidth,
                    rect.y + rect.height / 1.5f - Mathf.Max(1, baseLineWidth / 2),
                    rect.x + rect.width - frameWidth,
                    rect.y + rect.height / 1.5f + baseLineWidth / 2,
                    frameColor);
                vh.AddUIVertexQuad(quad);
            }
        }

        private void PopulateGraphNew(ref Rect rect, VertexHelper vh)
        {
            var dtmaxlog2 = Mathf.Log(targetFrameTime * 4, 2);
            var dtminlog2 = Mathf.Log(targetFrameTime / 2, 2);

            var capacity = (int)(rect.width / (minBarWidth + minSpaceBetweenBars));
            var count = Mathf.Min(PerformanceMonitorService.FrameBufferSize, capacity);
            var scale = capacity / (float)count;
            var space = minSpaceBetweenBars * scale;
            var height = rect.height;
            var width = minBarWidth * scale;
            var offset = Mathf.Max(0, PerformanceMonitorService.FrameBufferSize - capacity);

            for (int i = 0; i < count; ++i)
            {
                ref var frame = ref GetFrame(offset + i);
                
                var dtlog2 = Mathf.Log(frame.Time, 2);
                var factor = Mathf.Clamp01((dtlog2 - dtminlog2) / (dtmaxlog2 - dtminlog2));
                var color = TimeToColor(frame.Time);

                var x0 = rect.x + i * space + i * width;
                var y0 = rect.y;

                var quadHeight = factor * height;
                if (quadHeight < 2)
                {
                    quadHeight = 2;
                }

                FillQuad(ref quad, x0, y0, x0 + width, y0 + quadHeight, color);
                vh.AddUIVertexQuad(quad);
            }
        }

        private Color32 TimeToColor(float time)
        {
            if (time < colorSteps[0])
            {
                return colors[0];
            }

            for (int i = 1; i < colorSteps.Length; ++i)
            {
                if (time < colorSteps[i])
                {
                    var t = Mathf.InverseLerp(colorSteps[i - 1], colorSteps[i], time);
                    return Color32.LerpUnclamped(colors[i - 1], colors[i], t);
                }
            }

            return colors[3];
        }

        private static void FillQuad(ref UIVertex[] buffer, float x0, float y0, float x1, float y1, Color32 color)
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