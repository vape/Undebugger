using System;
using System.Collections.Generic;
using UnityEngine;

namespace Deszz.Undebugger.UI.Windows
{
    public interface IWindowContent
    {
        void AddingToWindow(Window window);
        void RemovedFromWindow(Window window);
    }

    public interface IWindowButtonsContainer
    {
        void AttachWindowButtons(WindowControlButton[] buttons);
        void DetachWindowButtons();
    }

    [RequireComponent(typeof(RectTransform))]
    public class Window : MonoBehaviour
    {
        public delegate void StateChangedDelegate(Window window, WindowState state);
        public delegate void CloseDelegate(Window window);
        public delegate void ContentChangedDelegate(Window window, RectTransform content);

        public event ContentChangedDelegate ContentChanged;

        public event StateChangedDelegate ChangingWindowState;
        public event StateChangedDelegate ChangedWindowState;

        public event CloseDelegate Closing;
        public event CloseDelegate Closed;

        public Guid UID
        {
            get
            {
                return uid;
            }
        }

        public RectTransform Rect
        {
            get
            {
                if (rect == null)
                {
                    rect = GetComponent<RectTransform>();
                }

                return rect;
            }
        }

        public WindowSystem WindowSystem
        {
            get
            {
                return windowSystem;
            }
        }

        [SerializeField]
        private WindowHeader header;
        [SerializeField]
        private RectTransform contentContainer;

        protected RectTransform content;

        private bool closing;
        private WindowState state;
        private IWindowButtonsContainer activeButtonsContainer;
        private WindowSystem windowSystem;
        private readonly Guid uid = Guid.NewGuid();
        private RectTransform rect;

        public void Initialize(WindowSystem windowSystem)
        {
            this.windowSystem = windowSystem;
        }

        private void OnDestroy()
        {
            if (!closing)
            {
                Closing?.Invoke(this);
                closing = true;
            }

            if (windowSystem != null)
            {
                windowSystem.NotifyWindowDestroying(this);
            }

            Closed?.Invoke(this);
        }

        public void Close()
        {
            Closing?.Invoke(this);
            closing = true;

            Destroy(gameObject);
        }

        public void SetState(WindowState state)
        {
            if (activeButtonsContainer != null)
            {
                activeButtonsContainer.DetachWindowButtons();
                activeButtonsContainer = null;
            }

            ChangingWindowState?.Invoke(this, state);

            this.state = state;

            var parentContainer = Rect.parent.GetComponent<RectTransform>();
            var contentButtonsHandler = content == null ? null : content.GetComponent<IWindowButtonsContainer>();

            if (state == WindowState.Maximized)
            {
                SetDraggable(false);
                SetResizable(false);

                if (contentButtonsHandler != null)
                {
                    SetHeaderEnabled(false);
                    contentButtonsHandler.AttachWindowButtons(CreateButtons());
                    activeButtonsContainer = contentButtonsHandler;
                }
                else
                {
                    SetHeaderEnabled(true);
                    header.AttachWindowButtons(CreateButtons());
                    activeButtonsContainer = header;
                }

                Expand(Rect);
            }
            else if (state == WindowState.Windowed)
            {
                SetHeaderEnabled(true);
                header.AttachWindowButtons(CreateButtons());
                activeButtonsContainer = header;

                Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentContainer.rect.width * 0.9f);
                Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentContainer.rect.height * 0.9f);
                Float(Rect);

                SetDraggable(true);
                SetResizable(true);
            }

            ChangedWindowState?.Invoke(this, this.state);
        }

        public void SetContent(RectTransform content)
        {
            DestroyContent();

            if (content != null)
            {
                var contentHandlers = content.GetComponentsInChildren<IWindowContent>();
                for (int i = 0; i < contentHandlers.Length; ++i)
                {
                    contentHandlers[i].AddingToWindow(this);
                }

                this.content = content;
                content.SetParent(contentContainer);
                Expand(content);
                ContentChanged?.Invoke(this, this.content);
            }
        }

        private WindowControlButton[] CreateButtons()
        {
            var presets = new List<(Sprite icon, Action action)>(2);

            presets.Add((windowSystem.Settings.GetIcon("close"), Close));

            if (state == WindowState.Maximized)
            {
                presets.Add((windowSystem.Settings.GetIcon("compress"), () => SetState(WindowState.Windowed)));
            }
            else if (state == WindowState.Windowed)
            {
                presets.Add((windowSystem.Settings.GetIcon("enlarge"), () => SetState(WindowState.Maximized)));
            }

            var views = new WindowControlButton[presets.Count];

            for (int i = 0; i < views.Length; ++i)
            {
                var view = Instantiate(windowSystem.Settings.ControlButtonTemplate, transform);
                view.Setup(presets[i].icon, presets[i].action);

                views[i] = view;
            }

            return views;
        }

        private void SetHeaderEnabled(bool value)
        {
            contentContainer.offsetMax = new Vector2(contentContainer.offsetMax.x, value ? -header.Height : 0f);
            header.gameObject.SetActive(value);
        }

        private void SetDraggable(bool value)
        {
            foreach (var handle in GetComponentsInChildren<WindowDragHandle>())
            {
                handle.enabled = value;
            }
        }

        private void SetResizable(bool value)
        {
            foreach (var handle in GetComponentsInChildren<WindowResizeHandle>())
            {
                handle.enabled = value;
            }
        }

        private void DestroyContent()
        {
            if (content != null)
            {
                var contentHandlers = content.GetComponentsInChildren<IWindowContent>();
                for (int i = 0; i < contentHandlers.Length; ++i)
                {
                    contentHandlers[i].RemovedFromWindow(this);
                }

                Destroy(content.gameObject);
                content = null;
                ContentChanged?.Invoke(this, null);
            }
        }

        private static void Expand(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.one;
        }

        private static void Float(RectTransform rect)
        {
            var size = rect.rect.size;

            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.offsetMin = size * -rect.pivot;
            rect.offsetMax = size * (Vector2.one - rect.pivot);
        }
    }
}