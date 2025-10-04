using TMPro;
using UnityEditor;
using UnityEngine;

namespace StateVisualController.Editor
{
	[CustomEditor(typeof(StateVisualActor_TMPText))]
	public sealed class StateVisualActor_TMPTextEditor : StateVisualControllerBaseEditor
	{
		protected override void DrawTargetField()
		{
			EditorGUI.BeginChangeCheck();
			var comp = (Component)EditorGUILayout.ObjectField("Target (TMP_Text)", GetTarget(), typeof(TMP_Text), true);
			if (EditorGUI.EndChangeCheck())
			{
				SetTarget(comp);
			}

			EditorGUILayout.HelpBox("지원 Asset: TextAsset(텍스트), TMP_FontAsset(폰트). 색상은 TMP_Text.color에 적용됩니다.", MessageType.Info);
		}

		private Component GetTarget()
		{
			var prop = serializedObject.FindProperty("target");
			return (Component)prop.objectReferenceValue;
		}

		private void SetTarget(Component value)
		{
			var prop = serializedObject.FindProperty("target");
			prop.objectReferenceValue = value;
		}
	}
}


