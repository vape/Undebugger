using Deszz.Undebugger.UI.Layout;
using System.Collections.Generic;
using UnityEngine;

namespace Deszz.Undebugger.UI.Windows
{
    [RequireComponent(typeof(RectTransform))]
    public class Window : MonoBehaviour
    {
        public delegate void WindowModeDelegate(Window window, WindowMode mode);
        public delegate void CloseDelegate(Window window);
        public delegate void ContentChangedDelegate(Window window, RectTransform content);

        public event ContentChangedDelegate ContentChanged;

        public event WindowModeDelegate ChangingWindowMode;
        public event WindowModeDelegate ChangedWindowMode;

        public event CloseDelegate Closing;
        public event CloseDelegate Closed;

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

        public WindowMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                if (mode != value)
                {
                    SetMode(value);
                }
            }
        }

        [SerializeField]
        public WindowHeader header;
        [SerializeField]
        private LayoutMaster layout;
        [SerializeField]
        private RectTransform contentContainer;
        [SerializeField]
        private WindowHeaderButton buttonTemplate;
        [SerializeField]
        private WindowResizeHandle[] resizeHandles;

        private RectTransform rect;
        private IWindowButtonsContainer activeButtonsContainer;
        private RectTransform content;
        private WindowMode mode;
        private bool closing;
        private float headerHeight;

        private void Awake()
        {
            headerHeight = header.Rect.rect.height;
        }

        private void OnDestroy()
        {
            if (!closing)
            {
                closing = true;
                Closing?.Invoke(this);
            }

            SetContent(null as RectTransform);
            WindowSystem.Instance.NotifyWindowDestroyed(this);
            Closed?.Invoke(this);
        }

        public void Close()
        {
            closing = true;
            Closing?.Invoke(this);

            Destroy(gameObject);
        }

        public void SetMode(WindowMode mode)
        {
            if (activeButtonsContainer != null)
            {
                activeButtonsContainer.RemoveAllWindowButtons();
                activeButtonsContainer = null;
            }

            ChangingWindowMode?.Invoke(this, mode);

            this.mode = mode;

            switch (this.mode)
            {
                case WindowMode.Windowed:
                    header.Draggable = true;
                    Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Rect.rect.width * 0.8f);
                    Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Rect.rect.height * 0.8f);
                    Rect.Float();
                    SetResizeHandlesActive(true);
                    break;

                case WindowMode.Maximized:
                    header.Draggable = false;
                    Rect.Expand();
                    SetResizeHandlesActive(false);
                    break;
            }

            if (this.mode == WindowMode.Maximized && content != null)
            {
                var contentButtonsContainer = content.GetComponent<IWindowButtonsContainer>();
                if (contentButtonsContainer != null)
                {
                    activeButtonsContainer = contentButtonsContainer;
                    SetHeaderVisible(false);
                }
            }

            if (activeButtonsContainer == null)
            {
                activeButtonsContainer = header;
                SetHeaderVisible(true);
            }

            activeButtonsContainer.AddWindowButtons(CreateButtons());

            ChangedWindowMode?.Invoke(this, this.mode);
        }

        private WindowHeaderButton[] CreateButtons()
        {
            var provider = content == null ? null : content.GetComponent<IWindowButtonsProvider>();
            var presets = new List<WindowButtonPreset>(6);

            presets.Add(new WindowButtonPreset() { Action = Close, Icon = UndebuggerSettings.Instance.Icons.GetIcon("close") });

            switch (mode)
            {
                case WindowMode.Windowed:
                    presets.Add(new WindowButtonPreset() { Action = () => SetMode(WindowMode.Maximized), Icon = UndebuggerSettings.Instance.Icons.GetIcon("enlarge") });
                    break;
                case WindowMode.Maximized:
                    presets.Add(new WindowButtonPreset() { Action = () => SetMode(WindowMode.Windowed), Icon = UndebuggerSettings.Instance.Icons.GetIcon("compress") });
                    break;
            }

            if (provider != null)
            {
                presets.AddRange(provider.GetWindowButtonPresets());
            }

            var views = new WindowHeaderButton[presets.Count];

            for (int i = 0; i < views.Length; ++i)
            {
                var view = Instantiate(buttonTemplate, transform);
                view.Setup(presets[i].Icon, presets[i].Action);

                views[i] = view;
            }

            return views;
        }

        public void SetHeaderVisible(bool value)
        {
            contentContainer.offsetMax = new Vector2(contentContainer.offsetMax.x, value ? -headerHeight : 0f);
            header.gameObject.SetActive(value);
        }

        public void SetResizeHandlesActive(bool value)
        {
            for (int i = 0; i < resizeHandles.Length; ++i)
            {
                resizeHandles[i].gameObject.SetActive(value);
            }
        }

        public void SetContent(MonoBehaviour behaviour)
        {
            var rect = behaviour.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            SetContent(rect);
        }

        public void SetContent(RectTransform contentObject)
        {
            if (content != null)
            {
                var contents = content.GetComponentsInChildren<IWindowContent>();
                for (int i = 0; i < contents.Length; ++i)
                {
                    contents[i].RemovingFromWindow();
                }

                Destroy(content.gameObject);
                content = null;
            }

            if (contentObject != null)
            {
                contentObject.SetParent(contentContainer);
                contentObject.Expand();

                if (layout != null)
                {
                    layout.SetDirty(LayoutDirtyFlag.All);
                    layout.ForceRefresh();
                }

                var contents = contentObject.GetComponentsInChildren<IWindowContent>();
                for (int i = 0; i < contents.Length; ++i)
                {
                    contents[i].AddingToWindow(this);
                }

                content = contentObject;
            }
        }
    }
}
