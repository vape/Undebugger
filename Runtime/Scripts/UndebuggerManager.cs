using Deszz.Undebugger.Builder;
using Deszz.Undebugger.UI.Menu;
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

        private MenuView menuView;
        private UndebuggerSceneManager sceneManager;

        private void Awake()
        {
            sceneManager = new UndebuggerSceneManager();
        }

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

            if (menuView != null && (triggerAction & ActivationTriggerAction.Close) != 0)
            {
                Destroy(menuView.gameObject);
            }
            else if (menuView == null && (triggerAction & ActivationTriggerAction.Open) != 0)
            {
                menuView = CreateMenu();
            }
        }

        private MenuView CreateMenu()
        {
            var model = ModelBuilder.Build();
            var settings = UndebuggerSettings.Instance;

            using (sceneManager.MakeActive())
            {
                var menu = GameObject.Instantiate(settings.MenuTemplate, sceneManager.GetSafeArea().Rect);
                menu.name = settings.MenuTemplate.name;
                menu.Load(model, new MenuContext() { Configuration = configuration, Settings = settings });

                return menu;
            }
        }
    }
}
