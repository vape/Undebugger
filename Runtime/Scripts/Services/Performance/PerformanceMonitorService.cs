using UnityEngine;

namespace Undebugger.Services.Performance
{
    internal class PerformanceMonitorService : MonoBehaviour
    {
        public const int FrameBufferSize = 120;

        public static PerformanceMonitorService Instance
        {
            get
            {
                return instance;
            }
        }

        private static PerformanceMonitorService instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (instance != null)
            {
                return;
            }

            var gameObject = new GameObject("Performance Monitor Service");
            gameObject.hideFlags = HideFlags.NotEditable;
            instance = gameObject.AddComponent<PerformanceMonitorService>();
            DontDestroyOnLoad(gameObject);
        }

        public float MeanFrameTime
        { get { return mean; } }
        public float TargetFrameTime
        { get { return target; } }

        public Frame[] frames = new Frame[FrameBufferSize];
        public int head;
        public int tail;
        public float mean;
        public float target;

        public ref Frame GetFrame(int index)
        {
            return ref frames[(head + index) % frames.Length];
        }

        private void Update()
        {
            head = tail <= head ? tail + 1 : head;

            var count = head > tail ? frames.Length - head + tail : tail - head;
            var time = Time.unscaledDeltaTime;
            
            mean = (time + ((count - 1) * mean)) / count;
            target = Application.targetFrameRate > 0 ? 1f / Application.targetFrameRate : mean;

            frames[tail].Time = time;
            frames[tail].Tier = target > time ? 0 : (time - target) / target;

            tail = (tail + 1) % frames.Length;
        }
    }
}
