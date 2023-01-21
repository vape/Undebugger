using Undebugger.Model;
using Undebugger.Model.Commands;
using Undebugger.UI.Layout;
using UnityEngine;

namespace Undebugger.UI.Menu.Commands
{
    public class CommandsGroupView : GroupView
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
                DestroyImmediate(pageView.gameObject);
                pageView = null;
            }

            if (page >= 0 && page < model.Pages.Count)
            {
                pageView = Instantiate(pageTemplate, pageContainer);
                pageView.Init(model.Pages[page], commandViewFactory);
            }

            for (int i = 0; i < tabButtons.Length; ++i)
            {
                tabButtons[i].SetSelected(i == page);
            }

            LayoutUtility.SetLayoutDirty(transform, LayoutDirtyFlag.All);
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
                        GameObject.Destroy(tabButtons[i].gameObject);
                    }
                }
            }

            if (tabButtons == null || tabButtons.Length != model.Pages.Count)
            {
                tabButtons = new TabButton[model.Pages.Count];
            }

            for (int i = 0; i < model.Pages.Count; ++i)
            {
                tabButtons[i] = Instantiate(tabButtonTemplate, tabButtonsContainer);
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
