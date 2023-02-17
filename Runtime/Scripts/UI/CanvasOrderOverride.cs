using Undebugger.Services.UI;
using UnityEngine;

namespace Undebugger.UI
{
    internal class CanvasOrderOverride : MonoBehaviour
    {
        [SerializeField]
        private int menuOrderOffset;

        private void OnEnable()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = UIService.CanvasOrder + menuOrderOffset;
            }
        }
    }
}
