#if (UNITY_EDITOR || DEBUG || UNDEBUGGER) && !UNDEBUGGER_DISABLE
#define UNDEBUGGER_ENABLED
#endif

using UnityEngine;

namespace Undebugger
{
    internal static class UndebuggerRoot
    {
        public const string Version = "1.0.0";

#if UNDEBUGGER_ENABLED

        public static readonly GameObject Object;
        public static readonly Transform Transform;

        static UndebuggerRoot()
        {
            Object = new GameObject("Undebugger");
            Object.hideFlags = HideFlags.NotEditable;
            Transform = Object.transform;
            GameObject.DontDestroyOnLoad(Object);
        }

        public static T CreateServiceInstance<T>(string name)
            where T : MonoBehaviour
        {
            var serviceObject = new GameObject(name);
            serviceObject.hideFlags = HideFlags.NotEditable;
            serviceObject.transform.SetParent(Transform);

            return serviceObject.AddComponent<T>();
        }

#endif

    }
}


