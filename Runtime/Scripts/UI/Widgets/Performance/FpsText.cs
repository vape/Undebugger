using System;
using Undebugger.Services.Performance;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Widgets.Performance
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    internal class FpsText : Graphic
    {
        private static UIVertex[] quad = new UIVertex[4];

        public override Texture mainTexture
        {
            get
            {
                return material.mainTexture;
            }
        }

        public override Material material
        {
            get
            {
                return font.material;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        [SerializeField]
        private Font font;
        [SerializeField]
        private float space = 10f;

        private int fps = -1;
        private UIVertex[] digits;

        protected override void OnEnable()
        {
            RebuildDigitsMesh();
            base.OnEnable();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            RebuildDigitsMesh();
            base.OnValidate();
        }
#endif

        private void Update()
        {
            var frametime = 0f;

            if (PerformanceMonitorService.Instance != null)
            {
                frametime = PerformanceMonitorService.Instance.MeanFrameTime;
            }
#if UNITY_EDITOR
            else if (!Application.isPlaying)
            {
                frametime = 0.0087654f;
            }
#endif

            var fps = (int)(1f / frametime);
            if (this.fps != fps)
            {
                this.fps = fps;
                SetVerticesDirty();
            }
        }

        private void RebuildDigitsMesh()
        {
            var settings = new TextGenerationSettings()
            {
                font = font,
                color = color,
                scaleFactor = 1f,
                updateBounds = true,
                generateOutOfBounds = true,
                textAnchor = TextAnchor.MiddleLeft
            };

            using (var generator = new TextGenerator(9))
            {
                generator.Populate("0123456789", settings);
                digits = generator.GetVerticesArray();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var rect = GetPixelAdjustedRect();
            PupulateDigitsMesh(ref rect, vh, fps);
        }

        private void PupulateDigitsMesh(ref Rect rect, VertexHelper vh, int value)
        {
            var count = 0;
            var __value = value;
            while (__value > 0)
            {
                __value /= 10;
                count++;
            }

            var offset = 0;
            while (value > 0)
            {
                var rem = value % 10;
                value /= 10;

                for (int k = 0; k < 4; ++k)
                {
                    quad[k] = digits[rem * 4 + k];
                    quad[k].position.x += rect.x + space * (count - offset - 1);
                }

                vh.AddUIVertexQuad(quad);
                offset++;
            }
        }
    }
}
