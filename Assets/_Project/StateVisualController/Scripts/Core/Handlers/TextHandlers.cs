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
    /// Text 내용 변경을 위한 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "TextContentData", menuName = "StateVisualController/Text Content Data")]
    public class TextContentData : ScriptableObject
    {
        [SerializeField] private string content = string.Empty;
        
        public string Content 
        { 
            get => content; 
            set => content = value ?? string.Empty; 
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
            if (targetComponent is Text text && data.Data is TextContentData textData)
            {
                text.text = textData.Content;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Text) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            if (stateData.Data == null)
            {
                stateData.Data = ScriptableObject.CreateInstance<TextContentData>();
            }
            
            var textData = stateData.Data as TextContentData;
            EditorGUI.BeginChangeCheck();
            textData.Content = EditorGUILayout.TextField("Text", textData.Content);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
