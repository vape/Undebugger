using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI.Elements
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    [RequireComponent(typeof(CanvasRenderer))]
    internal class QuadGraphic : Graphic
    { }
}
