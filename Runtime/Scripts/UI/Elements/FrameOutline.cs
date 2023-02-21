using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Elements
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    [RequireComponent(typeof(CanvasRenderer))]
    internal class FrameOutline : Graphic
    {
        private static UIVertex[] quad = new UIVertex[4];

        [SerializeField]
        public int width;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (width > 0)
            {
                var rect = GetPixelAdjustedRect();
                var color = (Color32)this.color;

                FillQuad(ref quad,
                    rect.x - width,
                    rect.y - width,
                    rect.x,
                    rect.y + rect.height + width,
                    color);
                vh.AddUIVertexQuad(quad);

                FillQuad(ref quad,
                    rect.x,
                    rect.y + rect.height,
                    rect.x + rect.width,
                    rect.y + rect.height + width,
                    color);
                vh.AddUIVertexQuad(quad);

                FillQuad(ref quad,
                    rect.x,
                    rect.y - width,
                    rect.x + rect.width,
                    rect.y,
                    color);
                vh.AddUIVertexQuad(quad);

                FillQuad(ref quad,
                    rect.x + rect.width,
                    rect.y - width,
                    rect.x + rect.width + width,
                    rect.y + rect.height + width,
                    color);
                vh.AddUIVertexQuad(quad);
            }
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
