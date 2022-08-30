using System;
using System.Collections.Generic;
using UnityEngine;

namespace Deszz.Undebugger.UI.Windows
{
    [RequireComponent(typeof(RectTransform))]
    public class WindowSystem : MonoBehaviour
    {
        public WindowSystemSettings Settings => settings;

        public RectTransform Container
        {
            get
            {
                return container;
            }
            set
            {
                container = value;
            }
        }

        [SerializeField]
        private RectTransform container;

        private bool initialized;
        private WindowSystemSettings settings;
        private Dictionary<Guid, Window> windows = new Dictionary<Guid, Window>();

        public void Initialize(WindowSystemSettings settings)
        {
            if (initialized)
            {
                return;
            }
           
            this.settings = settings;

            initialized = true;
        }

        public Window Create()
        {
            AssertInitialized();

            var window = Instantiate(settings.WindowTemplate, container);
            window.Initialize(this);

            windows.Add(window.UID, window);
            
            return window;
        }

        internal void NotifyWindowDestroying(Window window)
        {
            windows.Remove(window.UID);
        }

        private void AssertInitialized()
        {
            if (!initialized)
            {
                throw new InvalidOperationException("Call Initialize before using window system");
            }
        }
    }
}
