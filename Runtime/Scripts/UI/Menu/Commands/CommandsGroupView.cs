using Undebugger.Model;
using Undebugger.Model.Commands;
using Undebugger.UI.Layout;
using UnityEngine;

namespace Undebugger.UI.Menu.Commands
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class CommandsGroupView : GroupView
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

        private TabButton[] tabButtons;
        private CommandsGroupModel model;
        private CommandViewFactory commandViewFactory;
        private PageView pageView;

        public override void Load(MenuModel menuModel)
        {
            base.Load(menuModel);

            model = menuModel.Commands;
            commandViewFactory = new CommandViewFactory(commandTemplates);

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
                pageView.Init(model.Pages[page], commandViewFactory);
            }

            for (int i = 0; i < tabButtons.Length; ++i)
            {
                tabButtons[i].Selected = i == page;
            }

            ULayoutHelper.SetDirty(transform, ULayoutDirtyFlag.All);
        }

        private void CreateTabButtons()
        {
            if (tabButtons != null)
            {
                for (int i = 0; i < tabButtons.Length; ++i)
                {
                    if (tabButtons[i] != null)
                    {
                        tabButtons[i].Clicked -= TabButtonClickedHandler;
                        pool.AddOrDestroy(tabButtons[i]);
                    }
                }
            }

            if (tabButtons == null || tabButtons.Length != model.Pages.Count)
            {
                tabButtons = new TabButton[model.Pages.Count];
            }

            for (int i = 0; i < model.Pages.Count; ++i)
            {
                tabButtons[i] = pool.GetOrInstantiate(tabButtonTemplate, tabButtonsContainer);
                tabButtons[i].Init(model.Pages[i]);
                tabButtons[i].Clicked += TabButtonClickedHandler;
            }
        }

        private void TabButtonClickedHandler(PageModel model)
        {
            SetPage(model);
        }
    }
}
