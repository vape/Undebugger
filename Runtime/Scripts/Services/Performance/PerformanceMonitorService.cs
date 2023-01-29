using Undebugger.Utility;
using UnityEngine;
using UnityEngine.Profiling;

namespace Undebugger.Services.Performance
{
    internal class PerformanceMonitorService : MonoBehaviour
    {
        public const int FrameBufferSize = 210;
        public const int AverageOverNFrames = 30;

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
            gameObject.transform.SetParent(UndebuggerManager.Instance.transform);
            gameObject.hideFlags = HideFlags.NotEditable;
            instance = gameObject.AddComponent<PerformanceMonitorService>();
        }

        public long ReservedMemory
        {
            get
            {
                return Profiler.GetTotalReservedMemoryLong();
            }
        }

        public long AllocatedMemory
        {
            get
            {
                return Profiler.GetTotalAllocatedMemoryLong();
            }
        }

        public long MonoHeapSize
        {
            get
            {
                return Profiler.GetMonoHeapSizeLong();
            }
        }

        public long MonoUsageMemory
        {
            get
            {
                return Profiler.GetMonoUsedSizeLong();
            }
        }

        public float MeanFrameTime
        { get { return mean; } }
        public float TargetFrameTime
        { get { return target; } }

        private CircularBuffer<Frame> buffer = new CircularBuffer<Frame>(FrameBufferSize);
        public float mean;
        public float target;

        public ref Frame GetFrame(int index)
        {
            return ref buffer.Get(index);
        }

        private void Update()
        {
            var time = Time.unscaledDeltaTime;

            mean = (time + ((AverageOverNFrames - 1) * mean)) / AverageOverNFrames;
            target = Application.targetFrameRate > 0 ? 1f / Application.targetFrameRate : mean;

            buffer.PushFront(new Frame()
            {
                Time = time,
                Tier = target > time ? 0 : (time - target) / target
            });
        }
    }
}
