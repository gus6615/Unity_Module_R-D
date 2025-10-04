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
    /// TextMeshProUGUI 내용 변경을 위한 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "TextMeshProUGUIContentData", menuName = "StateVisualController/TextMeshProUGUI Content Data")]
    public class TextMeshProUGUIContentData : ScriptableObject
    {
        [SerializeField] private string content = string.Empty;
        
        public string Content 
        { 
            get => content; 
            set => content = value ?? string.Empty; 
        }
    }

    /// <summary>
    /// TextMeshProUGUI 색상 변경을 위한 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "TextMeshProUGUIColorData", menuName = "StateVisualController/TextMeshProUGUI Color Data")]
    public class TextMeshProUGUIColorData : ScriptableObject
    {
        [SerializeField] private Color color = Color.white;
        
        public Color Color 
        { 
            get => color; 
            set => color = value; 
        }
        
        private void OnValidate()
        {
            // Color 값이 유효한 범위 내에 있는지 확인
            color.r = Mathf.Clamp01(color.r);
            color.g = Mathf.Clamp01(color.g);
            color.b = Mathf.Clamp01(color.b);
            color.a = Mathf.Clamp01(color.a);
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
            if (targetComponent is TextMeshProUGUI text && data.Data is TextMeshProUGUIContentData textData)
            {
                text.text = textData.Content;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(TextMeshProUGUI) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            if (stateData.Data == null)
            {
                stateData.Data = ScriptableObject.CreateInstance<TextMeshProUGUIContentData>();
            }
            
            var textData = stateData.Data as TextMeshProUGUIContentData;
            EditorGUI.BeginChangeCheck();
            textData.Content = EditorGUILayout.TextField("Text", textData.Content);
            if (EditorGUI.EndChangeCheck())
            {
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
            if (targetComponent is TextMeshProUGUI text && data.Data is TextMeshProUGUIColorData colorData)
            {
                text.color = colorData.Color;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(TextMeshProUGUI) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            if (stateData.Data == null)
            {
                stateData.Data = ScriptableObject.CreateInstance<TextMeshProUGUIColorData>();
            }
            
            var colorData = stateData.Data as TextMeshProUGUIColorData;
            EditorGUI.BeginChangeCheck();
            colorData.Color = EditorGUILayout.ColorField("Color", colorData.Color);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
