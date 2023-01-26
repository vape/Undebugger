﻿using System.Collections.Generic;
using Undebugger.Services.Log;
using Undebugger.UI.Layout;
using UnityEngine;

namespace Undebugger.UI.Menu.Logs
{
    [RequireComponent(typeof(RectTransform))]
    internal class MessagesListView : LayoutRoot
    {
        [SerializeField]
        private RectTransform viewport;
        [SerializeField]
        private RectTransform rect;
        [SerializeField]
        private SmallMessageView template;
        [SerializeField]
        private float messageHeight;

        [Header("Expanded")]
        [SerializeField]
        private Transform expandedMessageContainer;
        [SerializeField]
        private ExpandedMessageView expandedMessageTemplate;

        private List<SmallMessageView> pool = new List<SmallMessageView>(capacity: 32);
        private Dictionary<int, SmallMessageView> views = new Dictionary<int, SmallMessageView>(capacity: 32);
        private HashSet<int> purgatory = new HashSet<int>(capacity: 32);
        private int visibleMinIndex;
        private int visibleMaxIndex;
        private ExpandedMessageView expandedMessage;

        private void OnEnable()
        {
            UpdateTotalVerticalSize();
            UpdateVisibleMessages();
            UpdateViews();

            if (rect.rect.height > viewport.rect.height)
            {
                ScrollToEnd();
            }

            LogStorageService.Instance.MessageAdded += MessageAddedHandler;
        }

        private void OnDisable()
        {
            LogStorageService.Instance.MessageAdded -= MessageAddedHandler;
        }

        private void MessageAddedHandler(LogMessage message)
        {
            UpdateTotalVerticalSize();

            if (rect.rect.height > viewport.rect.height && visibleMaxIndex > (LogStorageService.Instance.Count - 2))
            {
                ScrollToEnd();
            }
        }

        private void ScrollToEnd()
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -rect.rect.y - viewport.rect.height);
        }

        private void UpdateTotalVerticalSize()
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, messageHeight * LogStorageService.Instance.Count);
        }

        private void UpdateVisibleMessages()
        {
            var viewportMin = viewport.TransformPoint(viewport.rect.min).y - messageHeight;
            var viewportMax = viewport.TransformPoint(viewport.rect.max).y + messageHeight;

            var rectMin = rect.transform.InverseTransformPoint(0, viewportMin, 0).y;
            var rectMax = rect.transform.InverseTransformPoint(0, viewportMax, 0).y;

            visibleMaxIndex = Mathf.Min(LogStorageService.Instance.Count, Mathf.RoundToInt(rectMin / -messageHeight));
            visibleMinIndex = Mathf.Max(0, Mathf.RoundToInt(rectMax / -messageHeight));
        }

        private void UpdateViews()
        {
            purgatory.Clear();

            foreach (var kv in views)
            {
                purgatory.Add(kv.Key);
            }

            for (int i = visibleMinIndex; i < visibleMaxIndex; ++i)
            {
                ref var message = ref LogStorageService.Instance.GetMessage(i);

                if (!views.TryGetValue(message.Id, out var view))
                {
                    if (pool.Count > 0)
                    {
                        view = pool[pool.Count - 1];
                        view.gameObject.SetActive(true);
                        pool.RemoveAt(pool.Count - 1);
                    }
                    else
                    {
                        view = Instantiate(template, transform);
                    }
                    
                    view.Setup(in message);
                    view.Clicked += ExpandMessage;
                    views.Add(message.Id, view);
                }

                view.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, rect.rect.width);
                view.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, messageHeight * i, messageHeight);

                purgatory.Remove(message.Id);
            }

            foreach (var id in purgatory)
            {
                if (views.TryGetValue(id, out var view))
                {
                    view.Clicked -= ExpandMessage;
                    view.gameObject.SetActive(false);
                    pool.Add(view);
                    views.Remove(id);
                }
            }
        }

        private void ExpandMessage(int id)
        {
            if (expandedMessage != null)
            {
                CloseExpandedMessage();
            }

            if (!LogStorageService.Instance.TryFindById(id, out var message))
            {
                return;
            }

            expandedMessage = Instantiate(expandedMessageTemplate, expandedMessageContainer);
            expandedMessage.Setup(message);
            expandedMessage.CloseClicked += CloseExpandedMessage;
            expandedMessageContainer.gameObject.SetActive(true);

        }

        public void CloseExpandedMessage()
        {
            if (expandedMessage == null)
            {
                return;
            }

            expandedMessage.CloseClicked -= CloseExpandedMessage;
            Destroy(expandedMessage.gameObject);
            expandedMessageContainer.gameObject.SetActive(false);
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

            for (int i = visibleMinIndex; i < visibleMaxIndex; ++i)
            {
                ref var message = ref LogStorageService.Instance.GetMessage(i);

                if (views.TryGetValue(message.Id, out var view))
                {
                    view.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, rect.rect.width);
                    view.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, messageHeight * i, messageHeight);
                }
            }
        }
    }
}