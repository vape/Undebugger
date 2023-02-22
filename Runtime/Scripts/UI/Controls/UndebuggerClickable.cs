using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Undebugger.UI.Controls
{
    internal abstract class UndebuggerClickable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        protected enum ColorSchemeType
        {
            DarkMultiply,
            LightMultiply,
            LightAdditive
        }

        protected struct ColorScheme
        {
            public bool Additive;
            public Color PressedColor;
        }

        protected static readonly ColorScheme darkColorScheme = new ColorScheme()
        {
            PressedColor = new Color(0.8f, 0.8f, 0.8f, 1.0f)
        };

        protected static readonly ColorScheme lightColorScheme = new ColorScheme()
        {
            PressedColor = new Color(1.2f, 1.2f, 1.2f, 1.0f)
        };

        protected static readonly ColorScheme lightAdditiveColorScheme = new ColorScheme()
        {
            Additive = true,
            PressedColor = new Color(0.2f, 0.2f, 0.2f, 0.0f)
        };

        [SerializeField]
        private Graphic targetGraphic;
        [SerializeField]
        private ColorSchemeType colorScheme;

        private bool isPointerDown;
        private Color baseColor;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isPointerDown)
            {
                OnClick();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            isPointerDown = true;

            if (targetGraphic != null)
            {
                baseColor = targetGraphic.color;

                var colors = GetColorScheme();
                if (colors.Additive)
                {
                    targetGraphic.color += colors.PressedColor;
                }
                else
                {
                    targetGraphic.color *= colors.PressedColor;
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isPointerDown && targetGraphic != null)
            {
                targetGraphic.color = baseColor;
            }
        }

        protected virtual ColorScheme GetColorScheme()
        {
            switch (colorScheme)
            {
                case ColorSchemeType.LightMultiply:
                    return lightColorScheme;
                case ColorSchemeType.LightAdditive:
                    return lightAdditiveColorScheme;
                default:
                    return darkColorScheme;
            }
        }

        protected abstract void OnClick();
    }
}
