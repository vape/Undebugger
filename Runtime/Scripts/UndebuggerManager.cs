using Deszz.Undebugger.Builder;
using Deszz.Undebugger.UI.Menu;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Deszz.Undebugger
{
    public class UndebuggerManager : MonoBehaviour
    {
        public static UndebuggerManager Instance
        { get; private set; }

        private static UndebuggerConfiguration configuration = UndebuggerConfiguration.CreateDefault();

        public static UndebuggerConfiguration GetConfiguration()
        {
            return configuration;
        }

        public static void SetConfigration(UndebuggerConfiguration configuration)
        {
            UndebuggerManager.configuration = configuration;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Create()
        {
            var gameObject = new GameObject("Undebugger Manager");
            gameObject.hideFlags = HideFlags.NotEditable;
            DontDestroyOnLoad(gameObject);

            Instance = gameObject.AddComponent<UndebuggerManager>();
        }

        private SemaphoreSlim instanceAccess = new SemaphoreSlim(1, 1);
        private MenuView menu;
        private bool created;

        private void Update()
        {
            var triggerAction = ActivationTriggerAction.None;
            
            for (int i = 0; i < configuration.Triggers.Count; ++i)
            {
                if (configuration.Triggers[i].IsTriggered())
                {
                    triggerAction |= configuration.Triggers[i].Action;
                    break;
                }
            }

            if (triggerAction != ActivationTriggerAction.None)
            {
#pragma warning disable CS4014
                ToggleMenuAsync(triggerAction);
#pragma warning restore CS4014
            }
        }

        private void CloseRequestedHandler(MenuView view)
        {
#pragma warning disable CS4014
            TryCloseAsync();
#pragma warning restore CS4014
        }

        private async Task ToggleMenuAsync(ActivationTriggerAction action)
        {
            await instanceAccess.WaitAsync();

            try
            {
                if (!created && (action & ActivationTriggerAction.Open) != 0)
                {
                    menu = UndebuggerUtility.CreateMenu(ModelBuilder.Build(), configuration);
                    menu.CloseRequested += CloseRequestedHandler;

                    created = true;
                }
                else if (created && (action & ActivationTriggerAction.Close) != 0)
                {
                    menu.CloseRequested -= CloseRequestedHandler;
                    await UndebuggerUtility.DestroyMenuAsync(menu);
                    created = false;
                }
            }
            finally
            {
                instanceAccess.Release();
            }
        }

        private async Task TryCloseAsync()
        {
            await instanceAccess.WaitAsync();

            try
            {
                if (created)
                {
                    menu.CloseRequested -= CloseRequestedHandler;
                    await UndebuggerUtility.DestroyMenuAsync(menu);
                    created = false;
                }
            }
            finally
            {
                instanceAccess.Release();
            }
        }
    }
}
