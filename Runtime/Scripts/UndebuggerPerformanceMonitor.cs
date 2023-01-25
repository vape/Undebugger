using UnityEngine;

namespace Undebugger
{
    public class UndebuggerPerformanceMonitor : MonoBehaviour
    {
        public const int FrameTimeBufferSize = 120;

        public delegate void MessageAddedDelegate(LogMessage message);

        public static UndebuggerPerformanceMonitor Instance
        {
            get
            {
                return instance;
            }
        }

        private static UndebuggerPerformanceMonitor instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (instance != null)
            {
                return;
            }

            var gameObject = new GameObject("Undebugger Performance Monitor");
            gameObject.hideFlags = HideFlags.NotEditable;
            instance = gameObject.AddComponent<UndebuggerPerformanceMonitor>();
            DontDestroyOnLoad(gameObject);
        }

        public float[] FrametimeTier = new float[FrameTimeBufferSize];
        public float[] Frametimes = new float[FrameTimeBufferSize];
        public int FrametimesHead;
        public int FrametimesTail;
        public float MeanFrametime;
        public float FrametimeTarget;
        
        private int count;

        private void Update()
        {
            FrametimesHead = FrametimesTail <= FrametimesHead ? FrametimesTail + 1 : FrametimesHead;
            
            count = count >= Frametimes.Length ? Frametimes.Length : count + 1;

            var index = FrametimesTail;
            var time = Time.unscaledDeltaTime;
            var mean = (time + ((count - 1) * MeanFrametime)) / count;
            var target = Application.targetFrameRate > 0 ? 1f / Application.targetFrameRate : mean;
            var tier = Mathf.Max(0, (time - target) / target);

            Frametimes[index] = time;
            FrametimeTier[index] = tier;
            MeanFrametime = mean;
            FrametimeTarget = target;

            FrametimesTail = (FrametimesTail + 1) % Frametimes.Length;
        }
    }
}
