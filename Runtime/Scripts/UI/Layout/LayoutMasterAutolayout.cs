using UnityEngine;

namespace Deszz.Undebugger.UI.Layout
{
    [RequireComponent(typeof(LayoutMaster))]
    internal class LayoutMasterAutolayout : MonoBehaviour
    {
        [SerializeField]
        private LayoutMaster master;

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

            master.DoLayout();
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

        private void LateUpdate()
        {
            if (dirty)
            {
                master.SetDirty(LayoutDirtyFlag.Layout);
                dirty = false;
            }
        }

        protected static bool Equal(Resolution left, Resolution right)
        {
            return left.width == right.width && left.height == right.height && left.refreshRate == right.refreshRate;
        }
    }
}
