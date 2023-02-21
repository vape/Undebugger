using UnityEngine;

namespace Undebugger.UI.Layout
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    [RequireComponent(typeof(RectTransform))]
    internal class ULayoutElement : MonoBehaviour, IULayoutElement
    {
        public bool Ignore
        { get { return ignore; } }
        public float MinWidth
        { get { return minWidth; } }
        public float MinHeight
        { get { return minHeight; } }

        [SerializeField]
        private bool ignore;
        [SerializeField]
        private float minWidth = -1;
        [SerializeField]
        private float minHeight = -1;
    }
}
