using UnityEditor;
using UnityEngine;

namespace StateVisualController.Editor
{
	[CustomEditor(typeof(StateVisualActor_Animation))]
	public sealed class StateVisualActor_AnimationEditor : StateVisualControllerBaseEditor
	{
		protected override void DrawTargetField()
		{
			EditorGUI.BeginChangeCheck();
			var comp = (Component)EditorGUILayout.ObjectField("Target (Animation)", GetTarget(), typeof(Animation), true);
			if (EditorGUI.EndChangeCheck())
			{
				SetTarget(comp);
			}

			EditorGUILayout.HelpBox("지원 Asset: AnimationClip. 상태 전환 시 해당 클립을 재생합니다.", MessageType.Info);
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


