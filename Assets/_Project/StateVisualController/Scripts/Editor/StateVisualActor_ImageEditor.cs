using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace StateVisualController.Editor
{
    [CustomEditor(typeof(StateVisualActor_Image))]
    public class StateVisualActor_ImageEditor : StateVisualControllerBaseEditor
    {
        private SerializedProperty targetProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            targetProp = serializedObject.FindProperty("target");
        }

        protected override void DrawTargetField()
        {
            EditorGUI.BeginChangeCheck();
            var newTarget = (MonoBehaviour)EditorGUILayout.ObjectField(
                "Target (Image)",
                (MonoBehaviour)targetProp.objectReferenceValue,
                typeof(Image),
                true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.Update();
                targetProp.objectReferenceValue = newTarget;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}


