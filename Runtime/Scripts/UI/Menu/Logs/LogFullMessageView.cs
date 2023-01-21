using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Logs
{
    public class LogFullMessageView : MonoBehaviour
    {
        [SerializeField]
        private Text time;
        [SerializeField]
        private Text text;
        [SerializeField]
        private Text stacktrace;
        [SerializeField]
        private Sprite[] icons;
        [SerializeField]
        private Image icon;

        public void Setup(LogMessage message)
        {
            text.text = message.Message;
            icon.sprite = icons[(int)message.Type];
            time.text = message.Time.ToString("HH:mm:ss.ffff");
            stacktrace.text = message.StackTrace;
        }
    }
}
