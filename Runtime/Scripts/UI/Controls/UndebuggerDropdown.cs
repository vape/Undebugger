using System.Collections.Generic;
using Undebugger.Services.UI;
using Undebugger.UI.Layout;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Undebugger.UI.Controls
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class UndebuggerDropdown : UndebuggerClickable
    {
        [SerializeField]
        private Text label;
        [SerializeField]
        private GameObject container;
        [SerializeField]
        private RectTransform content;
        [SerializeField]
        private UndebuggerDropdownOption template;
        [SerializeField]
        private ULayoutNode layout;
        [SerializeField]
        private UnityEvent<int> selectionChanged;

        private bool shown;
        private string[] options;
        private List<UndebuggerDropdownOption> optionViews;
        private bool optionsDirty = true;
        private int selectedIndex;

        private void OnDisable()
        {
            Hide();
            ClearOptionViews();
        }

        public void Show()
        {
            if (optionsDirty)
            {
                RebuildOptionViews();
                optionsDirty = false;
            }

            container.gameObject.SetActive(true);

            if (!container.TryGetComponent<Canvas>(out var canvas))
            {
                canvas = container.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = UIService.CanvasOrder + 1;

                container.AddComponent<GraphicRaycaster>();
            }

            ULayoutHelper.SetDirty(transform, ULayoutDirtyFlag.All);
            shown = true;
        }

        public void Hide()
        {
            container.gameObject.SetActive(false);
            shown = false;
        }

        public void SetSelected(int index)
        {
            selectedIndex = index;

            if (optionViews != null)
            {
                for (int i = 0; i < optionViews.Count; ++i)
                {
                    optionViews[i].Selected = index == i;
                }
            }

            if (options != null)
            {
                label.text = options[selectedIndex];
            }
        }

        public void SetOptions(string[] options)
        {
            this.options = options;
            optionsDirty = true;

            if (optionViews == null)
            {
                optionViews = new List<UndebuggerDropdownOption>(capacity: options.Length);
            }

            label.text = options[selectedIndex];
        }

        protected override void OnClick()
        {
            if (shown)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void RebuildOptionViews()
        {
            ClearOptionViews();

            for (int i = 0; i < options.Length; ++i)
            {
                var instance = Instantiate(template, content);
                // TODO: test
                instance.gameObject.SetActive(true);
                instance.Clicked += OptionClickedHandler;
                instance.Selected = selectedIndex == i;
                instance.Setup(options[i], i);

                optionViews.Add(instance);
            }
        }

        private void ClearOptionViews()
        {
            foreach (var option in optionViews)
            {
                option.Clicked -= OptionClickedHandler;
                GameObject.DestroyImmediate(option.gameObject);
            }

            optionViews.Clear();
        }

        private void OptionClickedHandler(int index)
        {
            if (shown)
            {
                Hide();
            }

            selectionChanged?.Invoke(index);
        }
    }
}
