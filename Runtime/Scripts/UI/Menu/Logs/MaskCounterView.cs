using Undebugger.Services.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Logs
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class MaskCounterView : MonoBehaviour
    {
        [SerializeField]
        private LogTypeMask mask;
        [SerializeField]
        private Text text;

        private int previous = -1;

        private void Update()
        {
            var count = LogStorageService.Instance.GetTotalCount(mask);
            if (count != previous)
            {
                text.text = count.ToString();
                previous = count;
            }
        }
    }
}
