using Deszz.Undebugger.Model;
using Deszz.Undebugger.UI.Menu;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Deszz.Undebugger
{
    internal static class UndebuggerUtility
    {
        public static MenuView CreateMenu(MenuModel model, UndebuggerConfiguration configuration)
        {
            var settings = UndebuggerSettings.Instance;

            var activeEventSystem = GameObject.FindObjectOfType<EventSystem>();
            var currentScene = SceneManager.GetActiveScene();

            var debugMenuScene = SceneManager.CreateScene("Undebugger");
            SceneManager.SetActiveScene(debugMenuScene);

            var canvas = CreateCanvas();
            if (activeEventSystem == null)
            {
                CreateEventSystem();
            }

            var menu = GameObject.Instantiate(settings.MenuTemplate, canvas.transform);
            menu.name = settings.MenuTemplate.name;
            menu.Load(model, new MenuContext() { Configuration = configuration, Settings = settings });

            SceneManager.SetActiveScene(currentScene);

            return menu;
        }

        public static async Task DestroyMenuAsync(MenuView view)
        {
            var scene = view.gameObject.scene;
            GameObject.Destroy(view.gameObject);
            var completionSource = new TaskCompletionSource<bool>();
            SceneManager.UnloadSceneAsync(scene).completed += (_) => completionSource.SetResult(true);
            await completionSource.Task;
        }

        private static EventSystem CreateEventSystem()
        {
            var eventSystemObject = new GameObject("Event System");

            var eventSystem = eventSystemObject.AddComponent<EventSystem>();
            var inputModule = eventSystemObject.AddComponent<StandaloneInputModule>();

            return eventSystem;
        }

        private static Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("Canvas");

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.65f;

            canvasObject.AddComponent<GraphicRaycaster>();

            return canvas;
        }
    }
}
