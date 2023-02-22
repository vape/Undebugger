using Undebugger.Model.Status;
using Undebugger.UI.Layout;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Menu.Status
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class StatusSegmentView : MonoBehaviour, IULayoutElement
    {
        bool IULayoutElement.Ignore => false;
        float IULayoutElement.MinHeight => minHeight;
        float IULayoutElement.MinWidth => minWidth;

        [SerializeField]
        private Text title;
        [SerializeField]
        private Text text;
        [SerializeField]
        private GameObject foldButton;
        [SerializeField]
        private GameObject unfoldButton;
        [SerializeField]
        private float titleSize = 55;
        [SerializeField]
        private float minWidth = 750;

        private bool folded;
        private IStatusSegmentDriver driver;
        private float minHeight;

        private void OnEnable()
        {
            if (driver != null)
            {
                driver.Changed += DriverChangedHandler;
            }
        }

        private void OnDisable()
        {
            if (driver != null)
            {
                driver.Changed -= DriverChangedHandler;
            }
        }

        private void DriverChangedHandler()
        {
            RefreshText();
            UpdateSize();
        }

        public void Init(IStatusSegmentDriver driver)
        {
            if (this.driver != null)
            {
                this.driver.Changed -= DriverChangedHandler;
                this.driver = null;
            }

            this.driver = driver;
            this.driver.Changed += DriverChangedHandler;

            RefreshText();
            SetFoldout(Preferences.GetStatusSegmentFoldout(driver.PersistentId, defaultValue: false));
        }

        public void SetFoldout(bool value)
        {
            folded = value;

            text.gameObject.SetActive(!folded);
            foldButton.SetActive(!folded);
            unfoldButton.SetActive(folded);

            if (driver != null)
            {
                Preferences.SetStatusSegmentFoldout(driver.PersistentId, value);
            }

            UpdateSize();
        }

        private void UpdateSize()
        {
            var textHeight = folded ? 0 : text.preferredHeight;
            text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, textHeight);
            minHeight = titleSize + textHeight;
            ULayoutHelper.SetDirty(transform, ULayoutDirtyFlag.Layout);
        }

        private void RefreshText()
        {
            title.text = driver.Title;
            text.text = driver.Text;
        }
    }
}
