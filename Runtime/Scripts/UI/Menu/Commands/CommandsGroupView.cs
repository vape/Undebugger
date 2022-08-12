using Deszz.Undebugger.Model;
using Deszz.Undebugger.Model.Commands;
using UnityEngine;

namespace Deszz.Undebugger.UI.Menu.Commands
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

        private TabButton[] tabButtons;
        private CommandsGroupModel model;
        private CommandViewFactory commandViewFactory;
        private PageView pageView;
        private bool layoutDirty;

        private void SetLayoutDirty()
        {
            layoutDirty = true;
        }

        private void Update()
        {
            if (layoutDirty)
            {
                pageView.Layout();
                layoutDirty = false;
            }
        }

        private void OnRectTransformDimensionsChange()
        {
            SetLayoutDirty();
        }

        public override void Load(MenuModel menuModel, MenuContext menuViewContext)
        {
            base.Load(menuModel, menuViewContext);

            model = menuModel.Commands;
            commandViewFactory = new CommandViewFactory(menuViewContext.Settings.CommandTemplates);

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
                Destroy(pageView.gameObject);
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

            SetLayoutDirty();
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
