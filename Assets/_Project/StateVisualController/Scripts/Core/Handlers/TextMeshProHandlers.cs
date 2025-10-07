using System;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== TextMeshPro (3D Text) 관련 핸들러들 ====================
    
    /// <summary>
    /// TextMeshPro (3D Text) 컴포넌트의 내용을 변경하는 핸들러
    /// </summary>
    public class TextMeshProContentHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is TextMeshPro text)
            {
                text.text = data.TextData;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(TextMeshPro) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            EditorGUI.BeginChangeCheck();
            string newContent = stateData.TextData;
            newContent = EditorGUILayout.TextField("Text", newContent);
            
            if (EditorGUI.EndChangeCheck())
            {
                stateData.TextData = newContent;
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }

    /// <summary>
    /// TextMeshPro (3D Text) 컴포넌트의 색상을 변경하는 핸들러
    /// </summary>
    public class TextMeshProColorHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data { public Color color; }
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is TextMeshPro text)
            {
                var json = data.TextData;
                var parsed = string.IsNullOrEmpty(json)
                    ? new Data { color = Color.white }
                    : JsonUtility.FromJson<Data>(json);
                text.color = parsed.color;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(TextMeshPro) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            var data = string.IsNullOrEmpty(stateData.TextData)
                ? new Data { color = Color.white }
                : JsonUtility.FromJson<Data>(stateData.TextData);

            EditorGUI.BeginChangeCheck();
            var newColor = EditorGUILayout.ColorField("Color", data.color);
            if (EditorGUI.EndChangeCheck())
            {
                data.color = newColor;
                stateData.TextData = JsonUtility.ToJson(data);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
