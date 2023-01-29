using Undebugger.Model;
using Undebugger.UI.Layout;
using UnityEngine;

namespace Undebugger.UI.Menu
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField]
        private GroupButton groupButtonTemplate;
        [SerializeField]
        private RectTransform groupButtonContainer;
        [SerializeField]
        private Transform groupContainer;
        [SerializeField]
        private RectTransform groupButtonsWrapper;
        [SerializeField]
        private GroupView[] groupTemplates;
        [SerializeField]
        private MenuPool pool;
        
        private MenuModel model;
        private GroupButton[] groupButtons;
        private GroupView activeGroupView;

        public void Load(MenuModel model)
        {
            model.Sort();

            this.model = model;

            InitializeGroupButtons();
            SetActiveGroup(0);
        }

        public void SetActiveGroup(int group)
        {
            if (activeGroupView != null)
            {
                pool.AddOrDestroy(activeGroupView);
                activeGroupView = null;
            }

            if (group < 0 || group >= groupTemplates.Length)
            {
                for (int i = 0; i < groupButtons.Length; ++i)
                {
                    groupButtons[i].SetSelected(false);
                }

                return;
            }

            activeGroupView = pool.GetOrInstantiate(groupTemplates[group], groupContainer.transform);
            activeGroupView.Load(model);

            for (int i = 0; i < groupButtons.Length; ++i)
            {
                groupButtons[i].SetSelected(i == group);
            }

            LayoutUtility.SetLayoutDirty(groupContainer, LayoutDirtyFlag.All);
        }

        private void InitializeGroupButtons()
        {
            if (groupButtons != null)
            {
                for (int i = 0; i < groupButtons.Length; ++i)
                {
                    if (groupButtons[i] != null)
                    {
                        groupButtons[i].Clicked -= GroupButtonClickedHandler;
                        pool.AddOrDestroy(groupButtons[i]);
                    }
                }
            }

            if (groupButtons == null || groupButtons.Length != groupTemplates.Length)
            {
                groupButtons = new GroupButton[groupTemplates.Length];
            }

            for (int i = 0; i < groupTemplates.Length; ++i)
            {
                groupButtons[i] = pool.GetOrInstantiate(groupButtonTemplate, groupButtonContainer);
                groupButtons[i].Init(i, groupTemplates[i].GroupName);
                groupButtons[i].Clicked += GroupButtonClickedHandler;
            }

            LayoutUtility.SetLayoutDirty(groupButtonContainer, LayoutDirtyFlag.All);
        }

        private void GroupButtonClickedHandler(int index)
        {
            SetActiveGroup(index);
        }
    }
}
