using System.Collections.Generic;
using Undebugger.Services.Log;
using Undebugger.UI.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Logs
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    [RequireComponent(typeof(RectTransform))]
    internal class MessagesListView : MonoBehaviour, IULayoutNode, IPoolHandler
    {
        private const int InitialCapacity = 48;

        bool IULayoutNode.IsActive => isActiveAndEnabled;

        [SerializeField]
        private RectTransform viewport;
        [SerializeField]
        private RectTransform rect;
        [SerializeField]
        private SmallMessageView template;
        [SerializeField]
        private float messageHeight;
        [SerializeField]
        private ScrollStateHelper scrollStateHelper;
        [SerializeField]
        private ScrollRect scrollRect;

        [Header("Expanded")]
        [SerializeField]
        private Transform expandedMessageContainer;
        [SerializeField]
        private ExpandedMessageView expandedMessageTemplate;

        private MenuPool pool;
        private Dictionary<int, SmallMessageView> views;
        private HashSet<int> purgatory;
        private int visibleMinIndex;
        private int visibleMaxIndex;
        private ExpandedMessageView expandedMessage;
        private LogTypeMask mask = LogTypeMask.All;

        private void Awake()
        {
            views = new Dictionary<int, SmallMessageView>(capacity: InitialCapacity);

#if UNITY_2021_1_OR_NEWER
            purgatory = new HashSet<int>(capacity: InitialCapacity);
#else
            purgatory = new HashSet<int>();
#endif
        }

        private void OnEnable()
        {
            Rebuild();

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

        public void UsePool(MenuPool pool)
        {
            this.pool = pool;

            if (this.pool != null)
            {
                this.pool.Reserve(typeof(SmallMessageView), InitialCapacity);
            }
        }

        public void SetMask(LogTypeMask mask)
        {
            this.mask = mask;

            Rebuild();

            if (rect.rect.height > viewport.rect.height)
            {
                ScrollToEnd();
            }
        }

        public LogTypeMask GetMask()
        {
            return mask;
        }

        private void Rebuild()
        {
            UpdateTotalVerticalSize();
            UpdateVisibleMessages();
            UpdateViews();
        }

        private void MessageAddedHandler(in LogMessage message)
        {
            UpdateTotalVerticalSize();

            if (rect.rect.height > viewport.rect.height && visibleMaxIndex > (LogStorageService.Instance.GetCount(mask) - 2))
            {
                ScrollToEnd();
            }
        }

        private void ScrollToEnd()
        {
            if (scrollStateHelper.IsDragging)
            {
                return;
            }

            scrollRect.StopMovement();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -rect.rect.y - viewport.rect.height);
        }

        private void UpdateTotalVerticalSize()
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, messageHeight * LogStorageService.Instance.GetCount(mask));
        }

        private void UpdateVisibleMessages()
        {
            var viewportMin = viewport.TransformPoint(viewport.rect.min).y - messageHeight;
            var viewportMax = viewport.TransformPoint(viewport.rect.max).y + messageHeight;

            var rectMin = rect.transform.InverseTransformPoint(0, viewportMin, 0).y;
            var rectMax = rect.transform.InverseTransformPoint(0, viewportMax, 0).y;

            visibleMaxIndex = Mathf.Min(LogStorageService.Instance.GetCount(mask), Mathf.RoundToInt(rectMin / -messageHeight));
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
                ref var message = ref LogStorageService.Instance.GetMessage(mask, i);

                if (!views.TryGetValue(message.Id, out var view))
                {
                    view = pool.GetOrInstantiate(template, transform);
                    view.Setup(in message);
                    view.Clicked += ExpandMessage;

                    views.Add(message.Id, view);
                }

                view.SetViewIndex(i);
                view.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, rect.rect.width);
                view.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, messageHeight * i, messageHeight);

                purgatory.Remove(message.Id);
            }

            foreach (var id in purgatory)
            {
                if (views.TryGetValue(id, out var view))
                {
                    view.Clicked -= ExpandMessage;

                    pool.AddOrDestroy(view);
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

            expandedMessage = pool.GetOrInstantiate(expandedMessageTemplate, expandedMessageContainer);
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
            pool.AddOrDestroy(expandedMessage);

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

        void IULayoutNode.OnHierarchyRebuild()
        { }

        void IULayoutNode.OnLayoutRebuild()
        {
            for (int i = visibleMinIndex; i < visibleMaxIndex; ++i)
            {
                ref var message = ref LogStorageService.Instance.GetMessage(mask, i);

                if (views.TryGetValue(message.Id, out var view))
                {
                    view.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, rect.rect.width);
                    view.Rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, messageHeight * i, messageHeight);
                }
            }
        }
    }
}
