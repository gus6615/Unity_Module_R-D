using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    /// <summary>
    /// Animator 컴포넌트에서 문자열 stateData를 애니메이션 클립 이름으로 간주하여 재생
    /// </summary>
    public class AnimatorPlayClipHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Animator animator)
            {
                var clipName = data.TextData;
                if (!string.IsNullOrEmpty(clipName))
                {
                    // Animator에 등록된 상태 이름 또는 직접 클립 이름으로 재생 시도
                    animator.Play(clipName, 0, 0f);
                }
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Animator) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            EditorGUI.BeginChangeCheck();
            string newClipName = stateData.TextData;
            newClipName = EditorGUILayout.TextField("Clip Name", newClipName);
            if (EditorGUI.EndChangeCheck())
            {
                stateData.TextData = newClipName;
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}


