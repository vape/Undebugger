using Undebugger.Services.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Logs
{
    internal class MessagesCounterView : MonoBehaviour
    {
        [SerializeField]
        private Text errors;
        [SerializeField]
        private Text info;
        [SerializeField]
        private Text warnings;

        private int infoCount = -1;
        private int warningCount = -1;
        private int errorCount = -1;

        private void Update()
        {
            if (LogStorageService.Instance.TotalInfo != infoCount)
            {
                infoCount = LogStorageService.Instance.TotalInfo;
                info.text = infoCount.ToString();
            }

            if (LogStorageService.Instance.TotalWarnings != warningCount)
            {
                warningCount = LogStorageService.Instance.TotalWarnings;
                warnings.text = warningCount.ToString();
            }

            if (LogStorageService.Instance.TotalErrors != errorCount)
            {
                errorCount = LogStorageService.Instance.TotalErrors;
                errors.text = errorCount.ToString();
            }
        }
    }
}
