using Undebugger.UI;
using Undebugger.UI.Layout;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Undebugger
{
    internal class UndebuggerSceneManager
    {
        public struct ActivationToken : IDisposable
        {
            private Scene currentScene;
            private bool disposed;

            public ActivationToken(Scene undebuggerScene, Scene currentScene)
            {
                SceneManager.SetActiveScene(undebuggerScene);

                this.currentScene = currentScene;
                this.disposed = false;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                SceneManager.SetActiveScene(currentScene);
                disposed = true;
            }
        }

        private Scene scene;
        private Canvas canvas;
        private SafeArea safeArea;

        public ActivationToken MakeActive()
        {
            EnsureSceneLoaded();
            return new ActivationToken(scene, SceneManager.GetActiveScene());
        }

        public Canvas GetCanvas()
        {
            EnsureSceneLoaded();
            return canvas;
        }

        public SafeArea GetSafeArea()
        {
            EnsureSceneLoaded();
            return safeArea;
        }

        private void EnsureSceneLoaded()
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                scene = CreateScene(out canvas, out safeArea);
            }
        }

        private static Scene CreateScene(out Canvas canvas, out SafeArea safeArea)
        {
            var activeEventSystem = GameObject.FindObjectOfType<EventSystem>();
            var currentScene = SceneManager.GetActiveScene();

            var debugMenuScene = SceneManager.CreateScene("Undebugger");
            SceneManager.SetActiveScene(debugMenuScene);

            CreateCanvas(out canvas, out safeArea);

            if (activeEventSystem == null)
            {
                CreateEventSystem();
            }

            SceneManager.SetActiveScene(currentScene);

            return debugMenuScene;
        }

        public static async Task UnloadSceneAsync(Scene scene)
        {
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

        private static void CreateCanvas(out Canvas canvas, out SafeArea safeArea)
        {
            var canvasObject = new GameObject("Canvas");

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32766;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.65f;

            var safeAreaObject = new GameObject("Safe Area");
            var safeAreaRect = safeAreaObject.AddComponent<RectTransform>();
            safeAreaRect.SetParent(canvasObject.transform);
            safeAreaRect.Expand();
            safeArea = safeAreaObject.AddComponent<SafeArea>();

            canvasObject.AddComponent<GraphicRaycaster>();
        }
    }
}
