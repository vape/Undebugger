using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Elements
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    [RequireComponent(typeof(CanvasRenderer))]
    internal class HorizontalBar : Graphic
    {
        private static UIVertex[] quad = new UIVertex[4];

        public float Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = Mathf.Clamp01(value);
                SetVerticesDirty();
            }
        }

        [SerializeField]
        protected Color backgroundColor;
        [SerializeField]
        protected Color foregroundColor;
        [SerializeField]
        protected float padding;

        private float value;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var rect = GetPixelAdjustedRect();

            FillQuad(ref quad, rect.x, rect.y, rect.x + rect.width, rect.y + rect.height, backgroundColor);
            vh.AddUIVertexQuad(quad);

            var width = (rect.width - padding) * Mathf.Clamp01(value);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                width = (rect.width - padding) * 0.5f;
            }
#endif

            FillQuad(ref quad, rect.x + padding, rect.y + padding, rect.x + width, rect.y + rect.height - padding, foregroundColor);
            vh.AddUIVertexQuad(quad);
        }

        protected static void FillQuad(ref UIVertex[] buffer, float x0, float y0, float x1, float y1, Color color)
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
