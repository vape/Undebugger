using Deszz.Undebugger.UI.Layout;
using System.Collections.Generic;
using UnityEngine;

namespace Deszz.Undebugger.UI.Menu.Logs
{
    [RequireComponent(typeof(RectTransform))]
    internal class LogMessagesList : LayoutRoot
    {
        [SerializeField]
        private RectTransform viewport;
        [SerializeField]
        private RectTransform rect;
        [SerializeField]
        private LogShortMessageView template;
        [SerializeField]
        private float messageHeight;
        [SerializeField]
        private Transform fullMessageContainer;
        [SerializeField]
        private LogFullMessageView fullMessageTemplate;

        private List<LogMessage> messages;
        private Dictionary<int, LogShortMessageView> existingViews = new Dictionary<int, LogShortMessageView>(16);
        private HashSet<int> visibleMessages = new HashSet<int>(16);
        private HashSet<int> viewsToRemove = new HashSet<int>(16);

        private void OnEnable()
        {
            var count = UndebuggerLogsStorage.Instance.Count;

            if (messages == null)
            {
                messages = new List<LogMessage>((int)(count * 1.2f));
            }

            for (int i = 0; i < count; ++i)
            {
                AddMessage(UndebuggerLogsStorage.Instance.GetMessage(i));
            }

            UpdateTotalVerticalSize();
            UpdateVisibleMessages();
            UpdateViews();

            if (rect.rect.height > viewport.rect.height)
            {
                ScrollToEnd();
            }

            UndebuggerLogsStorage.Instance.MessageAdded += AddMessageAndUpdateSize;
        }

        private void OnDisable()
        {
            messages.Clear();

            UndebuggerLogsStorage.Instance.MessageAdded -= AddMessageAndUpdateSize;
        }

        private void AddMessageAndUpdateSize(LogMessage message)
        {
            AddMessage(message);
            UpdateTotalVerticalSize();

            if (rect.rect.height > viewport.rect.height && visibleMessages.Contains(messages.Count - 2))
            {
                ScrollToEnd();
            }
        }

        private void ScrollToEnd()
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -rect.rect.y - viewport.rect.height);
        }

        private void AddMessage(LogMessage message)
        {
            messages.Add(message);
        }

        private void UpdateTotalVerticalSize()
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, messageHeight * messages.Count);
        }

        private void UpdateVisibleMessages()
        {
            visibleMessages.Clear();

            var min = viewport.TransformPoint(viewport.rect.min).y - messageHeight;
            var max = viewport.TransformPoint(viewport.rect.max).y + messageHeight;

            for (int i = 0; i < messages.Count; ++i)
            {
                var y = rect.transform.TransformPoint(new Vector2(0, -messageHeight * i)).y;
                
                if (y >= min && y < max)
                {
                    visibleMessages.Add(i);
                }
            }
        }

        private void UpdateViews()
        {
            foreach (var key in existingViews.Keys)
            {
                if (!visibleMessages.Contains(key))
                {
                    viewsToRemove.Add(key);
                }
            }

            foreach (var index in viewsToRemove)
            {
                existingViews[index].Clicked -= MessageViewClicked;
                Destroy(existingViews[index].gameObject);
                existingViews.Remove(index);
            }

            viewsToRemove.Clear();

            foreach (var index in visibleMessages)
            {
                if (existingViews.ContainsKey(index))
                {
                    continue;
                }

                var view = Instantiate(template, transform);
                view.SetValue(messages[index], index);
                view.Clicked += MessageViewClicked;

                view.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, rect.rect.width);
                view.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, messageHeight * index, messageHeight);

                existingViews.Add(index, view);
            }
        }

        private void MessageViewClicked(int index)
        {
            if (fullMessageContainer.gameObject.activeSelf)
            {
                CloseFullMessage();
            }

            fullMessageContainer.gameObject.SetActive(true);

            var view = Instantiate(fullMessageTemplate, fullMessageContainer);
            view.Setup(messages[index]);
            view.transform.SetAsFirstSibling();
        }

        public void CloseFullMessage()
        {
            if (!fullMessageContainer.gameObject.activeSelf)
            {
                return;
            }

            var messageView = fullMessageContainer.GetComponentInChildren<LogFullMessageView>();
            if (messageView != null)
            {
                Destroy(messageView.gameObject);
            }

            fullMessageContainer.gameObject.SetActive(false);
        }

        private void Update()
        {
            UpdateVisibleMessages();
            UpdateViews();
        }

        private void OnValidate()
        {
            if (viewport == null)
            {
                viewport = transform.parent.GetComponent<RectTransform>();
            }

            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
            }
        }

        public override void DoLayout()
        {
            base.DoLayout();

            foreach (var kv in existingViews)
            {
                kv.Value.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, rect.rect.width);
                kv.Value.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, messageHeight * kv.Key, messageHeight);
            }
        }
    }
}
