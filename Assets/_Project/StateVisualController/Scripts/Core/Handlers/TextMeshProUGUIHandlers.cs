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
}
