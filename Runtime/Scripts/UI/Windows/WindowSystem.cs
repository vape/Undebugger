using System.Collections.Generic;
using UnityEngine;

namespace Deszz.Undebugger.UI.Windows
{
    public class WindowSystem : MonoBehaviour
    {
        public static WindowSystem Initialize(RectTransform root)
        {
            var system = root.gameObject.AddComponent<WindowSystem>();

            var container = new GameObject("Windows");
            var rect = container.AddComponent<RectTransform>();
            rect.SetParent(system.transform);
            rect.Expand();
            system.container = rect;

            return system;
        }

        public static WindowSystem Instance => instance;

        private static WindowSystem instance;

        public Camera Camera
        {
            get
            {
                if (cam == null)
                {
                    cam = Camera.main;
                }

                return cam;
            }
        }

        [SerializeField]
        private RectTransform container;

        private List<Window> windows = new List<Window>(4);
        private Camera cam;

        private void Awake()
        {
            instance = this;
        }

        public Window CreateWindow()
        {
            var window = Instantiate(UndebuggerSettings.Instance.WindowTemplate, container);
            windows.Add(window);
            return window;
        }

        internal void NotifyWindowDestroyed(Window window)
        {
            windows.Remove(window);
        }
    }
}
