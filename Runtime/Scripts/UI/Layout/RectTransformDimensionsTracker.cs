using System;
using UnityEngine;

namespace Undebugger.UI.Layout
{
    public class RectTransformDimensionsTracker : MonoBehaviour
    {
        public event Action DimensionsChanged;

        private void OnDestroy()
        {
            DimensionsChanged = null;
        }

        private void OnRectTransformDimensionsChange()
        {
            DimensionsChanged?.Invoke();
        }
    }
}
