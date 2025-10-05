using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== Text 관련 핸들러들 ====================
    
    /// <summary>
    /// Text 컴포넌트의 내용을 변경하는 핸들러
    /// </summary>
    public class TextContentHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Text text)
            {
                text.text = data.TextData;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Text) };

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
}
