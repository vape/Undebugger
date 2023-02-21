using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Elements
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class PointerQuad : Graphic
    {
        private static readonly Vector2 uv = new Vector2(0, 0);

        [SerializeField]
        private float size = 15;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);

            Color32 color32 = color;
            vh.Clear();
            vh.AddVert(new Vector3(v.x, v.y), color32, uv);
            vh.AddVert(new Vector3(v.x, v.w), color32, uv);
            vh.AddVert(new Vector3(v.z, v.w), color32, uv);
            vh.AddVert(new Vector3(v.z, v.y), color32, uv);

            vh.AddVert(new Vector3(v.z, r.y + r.height / 2), color32, uv);
            vh.AddVert(new Vector3(v.z + size, v.y + r.height / 2), color32, uv);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);

            vh.AddTriangle(2, 4, 5);
            vh.AddTriangle(3, 4, 5);
        }
    }
}
