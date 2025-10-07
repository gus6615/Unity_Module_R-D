using System;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== TMPText 관련 핸들러들 ====================
    
    /// <summary>
    /// TMPText 컴포넌트의 내용을 변경하는 핸들러
    /// </summary>
    public class TextMeshProUGUIContentHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is TextMeshProUGUI text)
            {
                text.text = data.TextData;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(TextMeshProUGUI) };

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
    /// TextMeshProUGUI 컴포넌트의 색상을 변경하는 핸들러
    /// </summary>
    public class TextMeshProUGUIColorHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data { public Color color; }
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is TextMeshProUGUI text)
            {
                var json = data.TextData;
                var parsed = string.IsNullOrEmpty(json)
                    ? new Data { color = Color.white }
                    : JsonUtility.FromJson<Data>(json);
                text.color = parsed.color;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(TextMeshProUGUI) };

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
