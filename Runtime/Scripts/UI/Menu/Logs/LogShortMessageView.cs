using System;
using Undebugger.Services.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Logs
{
    [RequireComponent(typeof(RectTransform))]
    public class LogShortMessageView : MonoBehaviour
    {
        public delegate void MessageClickedDelegate(int messageId);

        public event MessageClickedDelegate Clicked;

        public RectTransform Rect => rect;

        [SerializeField]
        private Sprite[] icons;
        [SerializeField]
        private Image icon;
        [SerializeField]
        private Image background;
        [SerializeField]
        private RectTransform rect;
        [SerializeField]
        private Text messageText;
        [SerializeField]
        private Color[] backColors;

        private int messageId;

        private void OnValidate()
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
            }
        }

        public void OnClick()
        {
            Clicked?.Invoke(messageId);
        }

        public void Setup(in LogMessage message)
        {
            messageId = message.Id;
            messageText.text = message.Message;
            icon.sprite = icons[(int)message.Type];
            background.color = backColors[message.Id % backColors.Length];
        }
    }
}
