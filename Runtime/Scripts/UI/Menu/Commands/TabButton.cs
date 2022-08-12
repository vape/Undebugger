﻿using Deszz.Undebugger.Model.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deszz.Undebugger.UI.Menu.Commands
{
    public class TabButton : MonoBehaviour
    {
        public delegate void ClickedDelegate(PageModel model);

        public event ClickedDelegate Clicked;

        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private Outline selectionOutline;

        private PageModel model;

        private void OnDestroy()
        {
            Clicked = null;
        }

        public void SetSelected(bool selected)
        {
            selectionOutline.enabled = selected;
        }

        public void Init(PageModel model)
        {
            this.model = model;

            nameText.text = model.Name == null ? "Unnamed" : model.Name;
        }

        public void Click()
        {
            if (model != null)
            {
                Clicked?.Invoke(model);
            }
        }
    }
}