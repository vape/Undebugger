using System;
using Undebugger.Services.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Logs
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class ExpandedMessageView : MonoBehaviour
    {
        public Action CloseClicked;

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

        private LogMessage message;

        public void Setup(LogMessage message)
        {
            this.message = message;

            text.text = message.Message;
            icon.sprite = icons[(int)message.Type];
            time.text = message.Time.ToString("HH:mm:ss.ffff");
            stacktrace.text = message.StackTrace;
        }

        public void OnCloseClicked()
        {
            CloseClicked?.Invoke();
        }

        public void SendByEmail()
        {
            var body = $"{message.Time} {message.Type}\n\n{message.Message}\n\n{message.StackTrace}\n";
            var url = $"mailto:?subject=LogMessage&body={System.Uri.EscapeUriString(body)}";

            Application.OpenURL(url);
        }
    }
}
