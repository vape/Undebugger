#if (UNITY_EDITOR || DEBUG || UNDEBUGGER) && !UNDEBUGGER_DISABLE
#define UNDEBUGGER_ENABLED
#endif

using System.Collections.Generic;
using Undebugger.Model;
using Undebugger.UI;
using Undebugger.UI.Layout;
using Undebugger.UI.Menu;
using Undebugger.UI.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Undebugger.Scripts.Services.UI
{
#if UNDEBUGGER_ENABLED

    internal class UIService : MonoBehaviour
    {
        public const int CanvasOrder = 32000;
        private const string MenuViewTemplateName = "Undebugger Menu View";

        public static UIService Instance
        {
            get
            {
                return instance;
            }
        }

        private static UIService instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (instance == null)
            {
                instance = UndebuggerRoot.CreateServiceInstance<UIService>("UI Service");
            }
        }

        public bool IsMenuOpen
        { get { return view != null && view.gameObject.activeSelf; } }

        private MenuView template;
        private MenuView view;
        private Canvas canvas;
        private SafeArea safeArea;
        private List<Widget> activeWidgets;

        public bool GetWidgetEnabled<T>()
            where T: Widget
        {
            if (activeWidgets == null)
            {
                return false;
            }

            for (int i = activeWidgets.Count - 1; i >= 0; --i)
            {
                if (activeWidgets[i].GetType() == typeof(T))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetWidgetEnabled<T>(bool value)
            where T: Widget
        {
            if (activeWidgets == null)
            {
                activeWidgets = new List<Widget>(capacity: 4);
            }

            if (!value)
            {
                for (int i = activeWidgets.Count - 1; i >= 0; --i)
                {
                    if (activeWidgets[i].GetType() == typeof(T))
                    {
                        Destroy(activeWidgets[i].gameObject);
                        activeWidgets.RemoveAt(i);
                        return;
                    }
                }
            }
            else
            {
                EnsureInitialized();

                if (!template.TryFindWidgetTemplate(typeof(T), out var widgetTemplate))
                {
                    return;
                }

                var instance = Instantiate(widgetTemplate, safeArea.Rect);
                instance.transform.SetAsFirstSibling();

                activeWidgets.Add(instance);
            }
        }

        public void OpenMenu(MenuModel model)
        {
            if (!IsMenuOpen)
            {
                if (view == null)
                {
                    EnsureInitialized();

                    view = GameObject.Instantiate(template, safeArea.Rect);
                    view.name = template.name;
                }
                else
                {
                    view.gameObject.SetActive(true);
                }
            }

            view.Load(model);
        }

        public void CloseMenu()
        {
            if (!IsMenuOpen)
            {
                return;
            }

            view.gameObject.SetActive(false);
        }

        private void EnsureInitialized()
        {
            if (template == null)
            {
                template = Resources.Load<MenuView>(MenuViewTemplateName);
            }

            if (canvas == null)
            {
                CreateCanvas(transform, out canvas, out safeArea);
            }
        }

        private static EventSystem CreateEventSystem(Transform parent)
        {
            var eventSystemObject = new GameObject("Event System");
            eventSystemObject.transform.SetParent(parent);

            var eventSystem = eventSystemObject.AddComponent<EventSystem>();
            var inputModule = eventSystemObject.AddComponent<StandaloneInputModule>();

            return eventSystem;
        }

        private static void CreateCanvas(Transform parent, out Canvas canvas, out SafeArea safeArea)
        {
            var canvasObject = new GameObject("Canvas");
            canvasObject.transform.SetParent(parent);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = CanvasOrder;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.65f;

            var safeAreaObject = new GameObject("Safe Area");
            var safeAreaRect = safeAreaObject.AddComponent<RectTransform>();
            safeAreaRect.SetParent(canvasObject.transform);
            safeAreaRect.Expand();
            safeArea = safeAreaObject.AddComponent<SafeArea>();

            canvasObject.AddComponent<GraphicRaycaster>();

            var activeEventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (activeEventSystem == null)
            {
                CreateEventSystem(parent);
            }
        }
    }

#else

    internal class UIService
    {
        public const int CanvasOrder = 32000;

        public static readonly UIService Instance = new UIService();

        public bool GetWidgetEnabled<T>()
            where T : Widget
        {
            return false;
        }

        public void SetWidgetEnabled<T>(bool value)
            where T : Widget
        { }

        public void CloseMenu()
        { }
    }

#endif
}