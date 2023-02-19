using UnityEngine;

namespace Undebugger.UI
{
    internal static class UIUtility
    {
        public static void Expand(this RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.one;
        }
    }
}
