using System;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Elements
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasRenderer))]
    internal class FastText : Graphic
    {
        public delegate char[] StringGetterDelegate(out int start, out int length);

        public enum VerticalAlignment : short
        {
            Top,
            Middle,
            Bottom
        }

        public enum HorizontalAlignment : short
        {
            Left,
            Center,
            Right
        }

        private const int CharsetOffset = 32;
        private const string Charset = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

        private static UIVertex[] TempQuad = new UIVertex[4];

        public override Texture mainTexture
        {
            get
            {
                if (material == null)
                {
                    return null;
                }

                return material.mainTexture;
            }
        }

        public override Material material
        {
            get
            {
                if (font == null)
                {
                    return null;
                }

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
        private float size = 16;
        [SerializeField]
        private VerticalAlignment verticalAlignment;
        [SerializeField]
        private HorizontalAlignment horizontalAlignment;
        [SerializeField]
        private string stubText;

#if UNITY_EDITOR
        private char[] stubTextBuffer;
#endif

        private StringGetterDelegate getter;
        private CharacterInfo[] glyphs = new CharacterInfo[Charset.Length];

        protected override void OnEnable()
        {
            CacheGlyphInfo();
            base.OnEnable();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (stubText != null)
                {
                    stubTextBuffer = stubText.ToCharArray();
                }

                if (getter == null)
                {
                    getter = new StringGetterDelegate((out int start, out int length) =>
                    {
                        start = 0;
                        length = stubTextBuffer.Length;

                        return stubTextBuffer;
                    });
                }
            }

            CacheGlyphInfo();
            base.OnValidate();
        }
#endif

        private void CacheGlyphInfo()
        {
            if (font == null)
            {
                return;
            }

            for (int i = 0; i < Charset.Length; ++i)
            {
                if (font.GetCharacterInfo(Charset[i], out var info, 0, FontStyle.Normal))
                {
                    glyphs[i] = info;
                }
                else
                {
                    glyphs[i] = default;
                }
            }
        }

        public void SetTextGetter(StringGetterDelegate getter)
        {
            this.getter = getter;
        }

        public void UpdateText()
        {
            SetVerticesDirty();
        }

        private Vector2 GetAlignmentFactor(VerticalAlignment vertical, HorizontalAlignment horizontal)
        {
            var factor = new Vector2();

            switch (verticalAlignment)
            {
                case VerticalAlignment.Top:
                    factor.y = 1.0f;
                    break;
                case VerticalAlignment.Middle:
                    factor.y = 0.5f;
                    break;
            }

            switch (horizontal)
            {
                case HorizontalAlignment.Right:
                    factor.x = 1.0f;
                    break;
                case HorizontalAlignment.Center:
                    factor.x = 0.5f;
                    break;
            }

            return factor;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (font == null || getter == null)
            {
                return;
            }

            var alignment = GetAlignmentFactor(verticalAlignment, horizontalAlignment);
            var rect = GetPixelAdjustedRect();
            var str = getter(out int offset, out int length);
            var scale = size / font.fontSize;
            // fasttext is used solely with monospaced fonts and text is always single line so its okay
            var lineWidth = length * glyphs[0].advance * scale;
            var color = (Color32)this.color;
            var height = font.lineHeight * scale;
            var ascent = font.ascent * scale;
            var px = -rect.width * rectTransform.pivot.x + ((rect.width - lineWidth) * alignment.x);
            var py = -rect.height * rectTransform.pivot.y + height - ascent + ((rect.height - height) * alignment.y);

            for (int i = 0; i < length; i++)
            {
                ref var glyph = ref glyphs[str[offset + i] - CharsetOffset];
                PopulateGlyph(TempQuad, px, py, scale, color, in glyph);
                vh.AddUIVertexQuad(TempQuad);
                px += glyph.advance * scale;
            }
        }

        private static void PopulateGlyph(UIVertex[] temp, float px, float py, float scale, Color32 color, in CharacterInfo glyph)
        {
            var gw = glyph.glyphWidth * scale;
            var gh = glyph.glyphHeight * scale;
            var hb = glyph.bearing * scale;
            var vb = glyph.minY * scale;

            temp[0].position.x = px + hb;
            temp[0].position.y = py + vb;
            temp[0].uv0 = glyph.uvBottomLeft;
            temp[0].color = color;

            temp[1].position.x = px + hb + gw;
            temp[1].position.y = py + vb;
            temp[1].uv0 = glyph.uvBottomRight;
            temp[1].color = color;

            temp[2].position.x = px + hb + gw;
            temp[2].position.y = py + vb + gh;
            temp[2].uv0 = glyph.uvTopRight;
            temp[2].color = color;

            temp[3].position.x = px + hb;
            temp[3].position.y = py + vb + gh;
            temp[3].uv0 = glyph.uvTopLeft;
            temp[3].color = color;
        }
    }
}
