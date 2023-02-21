using System;
using UnityEngine;

namespace Undebugger.UI
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class SafeAreaIgnore : MonoBehaviour
    {
        [Flags]
        public enum Side : int
        {
            None = 0,
            Left = 1,
            Top = 2,
            Right = 4,
            Bottom = 8,
            All = int.MaxValue
        }

        public Side ActiveSides
        {
            get
            {
                return activeSides;
            }
            set
            {
                activeSides = value;
                OnValidate();
            }
        }

        [SerializeField]
        private Side activeSides = Side.All;

        private RectTransform rect;
        private SafeArea safeArea;
        private bool hasSafeArea;

        private void OnEnable()
        {
            rect = GetComponent<RectTransform>();
            safeArea = transform.GetComponentInParent<SafeArea>();

            if (safeArea != null)
            {
                hasSafeArea = true;

                safeArea.DimensionsChanged += SafeAreaDimensionsChanged;
                SafeAreaDimensionsChanged(safeArea);
            }
        }

        private void OnDisable()
        {
            if (hasSafeArea)
            {
                safeArea.DimensionsChanged -= SafeAreaDimensionsChanged;
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                SafeAreaDimensionsChanged(safeArea);
            }
        }

        private void SafeAreaDimensionsChanged(SafeArea safeArea)
        {
            if (safeArea == null)
            {
                return;
            }

            var xmin = (activeSides & Side.Left) > 0 ? -safeArea.SafeAreaRect.position.x + safeArea.ScreenRect.position.x : 0;
            var ymin = (activeSides & Side.Bottom) > 0 ? -safeArea.SafeAreaRect.position.y + safeArea.ScreenRect.position.y : 0;

            var xmax = (activeSides & Side.Right) > 0 ? safeArea.ScreenRect.position.x + safeArea.ScreenRect.size.x - safeArea.SafeAreaRect.position.x : safeArea.SafeAreaRect.width;
            var ymax = (activeSides & Side.Top) > 0 ? safeArea.ScreenRect.position.y + safeArea.ScreenRect.size.y - safeArea.SafeAreaRect.position.y : safeArea.SafeAreaRect.height;

            xmin /= safeArea.SafeAreaRect.width;
            ymin /= safeArea.SafeAreaRect.height;
            xmax /= safeArea.SafeAreaRect.width;
            ymax /= safeArea.SafeAreaRect.height;

            rect.anchorMin = new Vector2(xmin, ymin);
            rect.anchorMax = new Vector2(xmax, ymax);
        }
    }
}
