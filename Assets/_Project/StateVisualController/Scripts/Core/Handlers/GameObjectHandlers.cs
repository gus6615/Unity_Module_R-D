using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== GameObject 관련 핸들러들 ====================
    
    /// <summary>
    /// GameObject의 활성화 상태를 변경하는 핸들러
    /// </summary>
    public class GameObjectActiveHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data
        {
            public bool active;
        }

        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Transform)
            {
                var json = data.TextData;
                var parsed = string.IsNullOrEmpty(json) 
                    ? new Data { active = true } 
                    : JsonUtility.FromJson<Data>(json);
                targetComponent.gameObject.SetActive(parsed.active);
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Transform), typeof(RectTransform) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            var data = string.IsNullOrEmpty(stateData.TextData)
                ? new Data { active = true }
                : JsonUtility.FromJson<Data>(stateData.TextData);

            EditorGUI.BeginChangeCheck();
            bool newActive = EditorGUILayout.Toggle("Active", data.active);
            if (EditorGUI.EndChangeCheck())
            {
                data.active = newActive;
                stateData.TextData = JsonUtility.ToJson(data);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
