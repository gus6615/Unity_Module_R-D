using System;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== TextMeshProUGUI 관련 데이터 클래스들 ====================
    
    /// <summary>
    /// TextMeshProUGUI 내용 변경을 위한 직렬화 가능한 데이터 클래스
    /// </summary>
    [Serializable]
    public class TextMeshProUGUIContentData
    {
        public string content;
        
        public TextMeshProUGUIContentData()
        {
            content = string.Empty;
        }
        
        public TextMeshProUGUIContentData(string content)
        {
            this.content = content ?? string.Empty;
        }
    }

    /// <summary>
    /// TextMeshProUGUI 색상 변경을 위한 직렬화 가능한 데이터 클래스
    /// </summary>
    [Serializable]
    public class TextMeshProUGUIColorData
    {
        public float r, g, b, a;
        
        public TextMeshProUGUIColorData()
        {
            r = g = b = a = 1f;
        }
        
        public TextMeshProUGUIColorData(Color color)
        {
            r = Mathf.Clamp01(color.r);
            g = Mathf.Clamp01(color.g);
            b = Mathf.Clamp01(color.b);
            a = Mathf.Clamp01(color.a);
        }
        
        public Color GetColor()
        {
            return new Color(r, g, b, a);
        }
    }

    // ==================== TMPText 관련 핸들러들 ====================
    
    /// <summary>
    /// TMPText 컴포넌트의 내용을 변경하는 핸들러
    /// </summary>
    public class TextMeshProUGUIContentHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is TextMeshProUGUI text && !string.IsNullOrEmpty(data.SerializedData))
            {
                try
                {
                    var textData = JsonUtility.FromJson<TextMeshProUGUIContentData>(data.SerializedData);
                    text.text = textData.content;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to apply text content state: {e.Message}");
                }
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(TextMeshProUGUI) };

        public override string SerializeData(StateHandlerData stateData)
        {
            if (targetComponent is TextMeshProUGUI text)
            {
                var textData = new TextMeshProUGUIContentData(text.text);
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
                    var textData = JsonUtility.FromJson<TextMeshProUGUIContentData>(jsonData);
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
            TextMeshProUGUIContentData textData = null;
            
            // 기존 데이터가 있으면 로드
            if (!string.IsNullOrEmpty(stateData.SerializedData))
            {
                try
                {
                    textData = JsonUtility.FromJson<TextMeshProUGUIContentData>(stateData.SerializedData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load text content data: {e.Message}");
                }
            }
            
            // 데이터가 없으면 새로 생성
            if (textData == null)
            {
                textData = new TextMeshProUGUIContentData();
            }
            
            EditorGUI.BeginChangeCheck();
            string newContent = textData.content;
            newContent = EditorGUILayout.TextField("Text", newContent);
            
            if (EditorGUI.EndChangeCheck())
            {
                var newTextData = new TextMeshProUGUIContentData(newContent);
                stateData.SerializedData = JsonUtility.ToJson(newTextData);
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
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is TextMeshProUGUI text && !string.IsNullOrEmpty(data.SerializedData))
            {
                try
                {
                    var colorData = JsonUtility.FromJson<TextMeshProUGUIColorData>(data.SerializedData);
                    text.color = colorData.GetColor();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to apply text color state: {e.Message}");
                }
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(TextMeshProUGUI) };

        public override string SerializeData(StateHandlerData stateData)
        {
            if (targetComponent is TextMeshProUGUI text)
            {
                var colorData = new TextMeshProUGUIColorData(text.color);
                return JsonUtility.ToJson(colorData);
            }
            return string.Empty;
        }

        public override void DeserializeData(string jsonData, StateHandlerData stateData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    var colorData = JsonUtility.FromJson<TextMeshProUGUIColorData>(jsonData);
                    // 데이터가 유효한지 확인
                    if (colorData != null)
                    {
                        stateData.SerializedData = jsonData;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to deserialize text color data: {e.Message}");
                }
            }
        }

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            TextMeshProUGUIColorData colorData = null;
            
            // 기존 데이터가 있으면 로드
            if (!string.IsNullOrEmpty(stateData.SerializedData))
            {
                try
                {
                    colorData = JsonUtility.FromJson<TextMeshProUGUIColorData>(stateData.SerializedData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load text color data: {e.Message}");
                }
            }
            
            // 데이터가 없으면 새로 생성
            if (colorData == null)
            {
                colorData = new TextMeshProUGUIColorData();
            }
            
            EditorGUI.BeginChangeCheck();
            Color newColor = colorData.GetColor();
            newColor = EditorGUILayout.ColorField("Color", newColor);
            
            if (EditorGUI.EndChangeCheck())
            {
                var newColorData = new TextMeshProUGUIColorData(newColor);
                stateData.SerializedData = JsonUtility.ToJson(newColorData);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
