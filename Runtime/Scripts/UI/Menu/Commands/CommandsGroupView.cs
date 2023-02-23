using System.Collections.Generic;
using Undebugger.Model;
using Undebugger.Model.Commands;
using Undebugger.Services.UI;
using Undebugger.UI.Layout;
using Undebugger.UI.Settings;
using UnityEngine;

namespace Undebugger.UI.Menu.Commands
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class CommandsGroupView : GroupView, ICommandsGroupContext
    {
        public override string GroupName => "Commands";

        [SerializeField]
        private TabButton tabButtonTemplate;
        [SerializeField]
        private RectTransform tabButtonsContainer;
        [Space]
        [SerializeField]
        private PageView pageTemplate;
        [SerializeField]
        private RectTransform pageContainer;
        [SerializeField]
        private CommandView[] commandTemplates;
        [SerializeField]
        private SettingsMenu settings;

        private List<TabButton> tabButtons;
        private CommandsGroupModel model;
        private CommandViewFactory commandViewFactory;
        private PageView pageView;

        private void Awake()
        {
            commandViewFactory = new CommandViewFactory(commandTemplates);
            tabButtons = new List<TabButton>(capacity: 8);

            settings.Setup(new ToggleSettingsOption()
            {
                Name = "Enable auto close",
                GetValue = () => Preferences.AutoCloseEnabled,
                SetValue = (value) => Preferences.AutoCloseEnabled = value
            });
        }

        public override void Load(MenuModel menuModel)
        {
            base.Load(menuModel);

            model = menuModel.Commands;

            CreateTabButtons();
            SetPage(0);
        }

        public void SetPage(PageModel model)
        {
            var index = this.model.Pages.IndexOf(model);
            if (index >= 0)
            {
                SetPage(index);
            }
        }

        public void SetPage(int page)
        {
            if (pageView != null)
            {
                pool.AddOrDestroy(pageView);
                pageView = null;
            }

            if (page >= 0 && page < model.Pages.Count)
            {
                pageView = pool.GetOrInstantiate(pageTemplate, pageContainer);
                pageView.Init(this, model.Pages[page], commandViewFactory);
            }

            for (int i = 0; i < tabButtons.Count; ++i)
            {
                tabButtons[i].Selected = i == page;
            }

            ULayoutHelper.SetDirty(transform, ULayoutDirtyFlag.All);
        }

        private void CreateTabButtons()
        {
            if (tabButtons != null)
            {
                for (int i = 0; i < tabButtons.Count; ++i)
                {
                    if (tabButtons[i] != null)
                    {
                        tabButtons[i].Clicked -= TabButtonClickedHandler;
                        pool.AddOrDestroy(tabButtons[i]);
                    }
                }

                tabButtons.Clear();
            }

            for (int i = 0; i < model.Pages.Count; ++i)
            {
                var tabButton = pool.GetOrInstantiate(tabButtonTemplate, tabButtonsContainer);
                tabButton.Init(model.Pages[i]);
                tabButton.Clicked += TabButtonClickedHandler;

                tabButtons.Add(tabButton);
            }
        }

        private void TabButtonClickedHandler(PageModel model)
        {
            SetPage(model);
        }

        void ICommandsGroupContext.TryCloseOnAction()
        {
            if (!Preferences.AutoCloseEnabled)
            {
                return;
            }

            UIService.Instance.CloseMenu();
        }
    }
}
