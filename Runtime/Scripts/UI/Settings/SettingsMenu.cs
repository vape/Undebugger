using System;
using System.Collections.Generic;
using Undebugger.UI.Layout;
using UnityEngine;

namespace Undebugger.UI.Settings
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class SettingsMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject background;
        [SerializeField]
        private RectTransform optionsContainer;
        [SerializeField]
        private ToggleSettingsOptionView toggleTemplate;
        [SerializeField]
        private ButtonSettingsOptionView buttonTemplate;

        private List<SettingsOptionView> optionViews = new List<SettingsOptionView>(capacity: 4);
        private bool shown;

        private void Awake()
        {
            Hide();
        }

        private void OnDisable()
        {
            Hide();
        }

        public void Setup(params SettingsOption[] options)
        {
            foreach (var view in optionViews)
            {
                Destroy(view.gameObject);
            }

            optionViews.Clear();

            for (int i = 0; i < options.Length; ++i)
            {
                switch (options[i])
                {
                    case ToggleSettingsOption toggleOption:
                        {
                            var instance = Instantiate(toggleTemplate, optionsContainer);
                            instance.Setup(toggleOption);
                            optionViews.Add(instance);
                        }
                        break;

                    case ButtonSettingsOption buttonOption:
                        {
                            var instance = Instantiate(buttonTemplate, optionsContainer);
                            instance.Setup(buttonOption);
                            optionViews.Add(instance);
                        }
                        break;

                    default:
                        throw new Exception("Unknown option type: " + options[i].GetType());
                }
            }
        }

        public void ToggleMenu()
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

        public void Show()
        {
            shown = true;

            background.gameObject.SetActive(true);
            optionsContainer.gameObject.SetActive(true);

            foreach (var option in optionViews)
            {
                option.Refresh();
            }

            ULayoutHelper.SetDirty(transform, ULayoutDirtyFlag.All);
        }

        public void Hide()
        {
            shown = false;

            background.gameObject.SetActive(false);
            optionsContainer.gameObject.SetActive(false);
        }
    }
}
