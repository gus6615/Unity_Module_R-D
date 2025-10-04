using UnityEditor;
using UnityEngine;

namespace StateVisualController.Editor
{
	[CustomEditor(typeof(StateVisualActor_AnimatorEditor))]
	public sealed class AnimatorStateActorEditor : StateVisualControllerBaseEditor
	{
		protected override void DrawTargetField()
		{
			EditorGUI.BeginChangeCheck();
			var comp = (Component)EditorGUILayout.ObjectField("Target (Animator)", GetTarget(), typeof(Animator), true);
			if (EditorGUI.EndChangeCheck())
			{
				SetTarget(comp);
			}

			EditorGUILayout.HelpBox("지원 Asset: RuntimeAnimatorController, AnimationClip(컨트롤러 기반 재생).", MessageType.Info);
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


