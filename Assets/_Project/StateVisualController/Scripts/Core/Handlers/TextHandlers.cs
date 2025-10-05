using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== Text 관련 데이터 클래스들 ====================
    
    /// <summary>
    /// Text 내용 변경을 위한 직렬화 가능한 데이터 클래스
    /// </summary>
    [Serializable]
    public class TextContentData
    {
        public string content;
        
        public TextContentData()
        {
            content = string.Empty;
        }
        
        public TextContentData(string content)
        {
            this.content = content ?? string.Empty;
        }
    }

    // ==================== Text 관련 핸들러들 ====================
    
    /// <summary>
    /// Text 컴포넌트의 내용을 변경하는 핸들러
    /// </summary>
    public class TextContentHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Text text && !string.IsNullOrEmpty(data.SerializedData))
            {
                try
                {
                    var textData = JsonUtility.FromJson<TextContentData>(data.SerializedData);
                    text.text = textData.content;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to apply text content state: {e.Message}");
                }
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Text) };

        public override string SerializeData(StateHandlerData stateData)
        {
            if (targetComponent is Text text)
            {
                var textData = new TextContentData(text.text);
                return JsonUtility.ToJson(textData);
            }
            return string.Empty;
        }

        public override void DeserializeData(string jsonData, StateHandlerData stateData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    var textData = JsonUtility.FromJson<TextContentData>(jsonData);
                    // 데이터가 유효한지 확인
                    if (textData != null)
                    {
                        stateData.SerializedData = jsonData;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to deserialize text content data: {e.Message}");
                }
            }
        }

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            TextContentData textData = null;
            
            // 기존 데이터가 있으면 로드
            if (!string.IsNullOrEmpty(stateData.SerializedData))
            {
                try
                {
                    textData = JsonUtility.FromJson<TextContentData>(stateData.SerializedData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load text content data: {e.Message}");
                }
            }
            
            // 데이터가 없으면 새로 생성
            if (textData == null)
            {
                textData = new TextContentData();
            }
            
            EditorGUI.BeginChangeCheck();
            string newContent = textData.content;
            newContent = EditorGUILayout.TextField("Text", newContent);
            
            if (EditorGUI.EndChangeCheck())
            {
                var newTextData = new TextContentData(newContent);
                stateData.SerializedData = JsonUtility.ToJson(newTextData);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
