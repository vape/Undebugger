#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_TVOS)
#define MOBILE_PLATFORM
#endif

#if !UNITY_EDITOR && !MOBILE_PLATFORM
#define VSYNC_SUPPORTED
#endif

using Undebugger.Utility;
using UnityEngine;
using UnityEngine.Profiling;

namespace Undebugger.Services.Performance
{
    internal class PerformanceMonitorService : MonoBehaviour
    {
        public const int FrameBufferSize = 210;
        public const int MeanOverNFrames = FrameBufferSize / 2;
        public const int SmoothOverNFrames = 20;

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
            if (instance == null)
            {
                instance = UndebuggerRoot.CreateServiceInstance<PerformanceMonitorService>("Performance Monitor Service");
            }
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

        public float SmoothedFrameTime
        { get { return smoothed; } }
        public float MeanFrameTime
        { get { return mean; } }
        public float TargetFrameTime
        { get { return target; } }

        private CircularBuffer<Frame> buffer = new CircularBuffer<Frame>(FrameBufferSize);
        private float smoothed;
        private float mean;
        private float target;

        public ref Frame GetFrame(int index)
        {
            return ref buffer.Get(index);
        }

        private void Update()
        {
            var time = Time.unscaledDeltaTime;

            smoothed = (time + ((SmoothOverNFrames - 1) * smoothed)) / SmoothOverNFrames;
            mean = (time + (MeanOverNFrames - 1) * mean) / MeanOverNFrames;

#if VSYNC_SUPPORTED
            if (QualitySettings.vSyncCount > 0)
            {
                target = 1f / (Screen.currentResolution.refreshRate / QualitySettings.vSyncCount);
            }
            else 
#endif
            if (Application.targetFrameRate > 0)
            {
#if MOBILE_PLATFORM 
                target = 1f / Mathf.Min(Application.targetFrameRate, Screen.currentResolution.refreshRate);
#else
                target = 1f / Application.targetFrameRate;
#endif
            }
            else
            {
                target = mean;
            }

            buffer.PushFront(new Frame(time));
        }
    }
}
