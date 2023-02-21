using Undebugger.Services.UI;
using Undebugger.UI.Widgets.Performance;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Status.Performance
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class TogglePerformanceWidgetButton : MonoBehaviour
    {
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Sprite pin;
        [SerializeField]
        private Sprite unpin;

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            icon.sprite = UIService.Instance.GetWidgetEnabled<PerformanceWidget>() ? unpin : pin;
        }

        public void OnClick()
        {
            if (UIService.Instance != null)
            {
                if (UIService.Instance.GetWidgetEnabled<PerformanceWidget>())
                {
                    UIService.Instance.SetWidgetEnabled<PerformanceWidget>(value: false);
                }
                else
                {
                    UIService.Instance.SetWidgetEnabled<PerformanceWidget>(value: true);
                }
            }

            Refresh();
        }
    }
}
