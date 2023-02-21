using UnityEngine;
using UnityEngine.UI;

namespace Undebugger.UI
{
#if !UNDEBUGGER_INTERNAL
    [AddComponentMenu("")]
#endif
    [RequireComponent(typeof(CanvasRenderer))]
    internal class NonDrawingGraphic : Graphic
    {
        public override void SetMaterialDirty() { return; }
        public override void SetVerticesDirty() { return; }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}

#if UNITY_EDITOR

namespace Undebugger.UI.Editor
{
    using UnityEditor;
    using UnityEditor.UI;

    [CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic), false)]
    internal class NonDrawingGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_Script, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif