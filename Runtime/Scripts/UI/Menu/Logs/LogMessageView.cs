using System;
using UnityEngine;
using UnityEngine.UI;

namespace Deszz.Undebugger.UI.Menu.Logs
{
    [RequireComponent(typeof(RectTransform))]
    public class LogMessageView : MonoBehaviour
    {
        public event Action<int> Clicked;

        public RectTransform Rect => rect;

        [SerializeField]
        private Text time;
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

        private int index;

        private void OnValidate()
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
            }
        }

        public void OnClick()
        {
            Clicked?.Invoke(index);
        }

        public void SetValue(LogMessage message, int index)
        {
            this.index = index;

            messageText.text = message.Message;
            icon.sprite = icons[(int)message.Type];
            background.color = backColors[index % backColors.Length];
            time.text = message.Time.ToString("HH:mm:ss.ffff");
        }
    }
}
