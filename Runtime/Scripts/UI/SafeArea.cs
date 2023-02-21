using UnityEngine;

namespace Undebugger.UI
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class SafeArea : MonoBehaviour
    {
        public delegate void DimentionsChangedDelegate(SafeArea safeArea);

        public event DimentionsChangedDelegate DimensionsChanged;

        public RectTransform Rect => rect;

        public virtual Rect SafeAreaRect => Screen.safeArea;
        public virtual Rect ScreenRect => canvas.pixelRect;

        private ScreenOrientation lastOrientation;
        private Resolution lastResolution;
        private Rect lastSafeArea;
        private bool dirty;

        protected Canvas canvas;
        protected RectTransform rect;

        protected virtual void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            rect = GetComponent<RectTransform>();

            lastOrientation = Screen.orientation;
            lastResolution = Screen.currentResolution;
            lastSafeArea = Screen.safeArea;

            OnSafeAreaChanged();
        }

        protected void SetDirty()
        {
            dirty = true;
        }

        protected void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        protected virtual void Update()
        {
            if (Application.isMobilePlatform && Screen.orientation != lastOrientation)
            {
                lastOrientation = Screen.orientation;
                SetDirty();
            }

            if (Screen.safeArea != lastSafeArea)
            {
                lastSafeArea = Screen.safeArea;
                SetDirty();
            }

            if (!Equal(Screen.currentResolution, lastResolution))
            {
                lastResolution = Screen.currentResolution;
                SetDirty();
            }
        }

        protected virtual void LateUpdate()
        {
            if (dirty)
            {
                OnSafeAreaChanged();
                dirty = false;

                DimensionsChanged?.Invoke(this);
            }
        }

        protected virtual void OnSafeAreaChanged()
        {
            ApplySafeArea(canvas, rect, SafeAreaRect);
        }

        protected static bool Equal(Resolution left, Resolution right)
        {
            return left.width == right.width && left.height == right.height && left.refreshRate == right.refreshRate;
        }

        protected static void ApplySafeArea(Canvas canvas, RectTransform rect, Rect area)
        {
            var anchorMin = area.position;
            var anchorMax = area.position + area.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
        }
    }
}
