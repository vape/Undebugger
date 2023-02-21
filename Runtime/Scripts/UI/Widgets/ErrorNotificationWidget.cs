using Undebugger.Model;
using Undebugger.Services.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Widgets
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    internal class ErrorNotificationWidget : Widget, IDebugMenuHandler
    {
        [SerializeField]
        private Text text;
        [SerializeField]
        private CanvasGroup alphaGroup;
        [SerializeField]
        private float showDuration = 10;

        private int unreadErrors;
        private float showTimeLeft;
        private float alpha;

        private void Awake()
        {
            alpha = 0;
            alphaGroup.alpha = alpha;
        }

        private void OnEnable()
        {
            LogStorageService.Instance.MessageAdded += MessageAddedHandler;
        }

        private void OnDisable()
        {
            LogStorageService.Instance.MessageAdded -= MessageAddedHandler;
        }

        private void MessageAddedHandler(in LogMessage message)
        {
            switch (message.Type)
            {
                case LogType.Exception:
                case LogType.Error:
                case LogType.Assert:
                    unreadErrors++;
                    text.text = message.Message;
                    showTimeLeft = showDuration;
                    break;
            }
        }

        private void Update()
        {
            showTimeLeft -= Time.unscaledDeltaTime;

            var newAlpha = Mathf.Clamp01(showTimeLeft);
            if (newAlpha != alpha)
            {
                alpha = newAlpha;
                alphaGroup.alpha = alpha;
            }
        }

        public void OnBuildingModel(MenuModel model)
        {
            showTimeLeft = 0;

            if (unreadErrors > 0)
            {
                unreadErrors = 0;
                model.StartGroup = BuiltinGroup.Log;
            }
        }
    }
}
