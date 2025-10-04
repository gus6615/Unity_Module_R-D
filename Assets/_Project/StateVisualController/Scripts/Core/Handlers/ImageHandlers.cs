using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== Image 관련 핸들러들 ====================
    
    /// <summary>
    /// Image 컴포넌트의 Sprite를 변경하는 핸들러
    /// </summary>
    public class ImageSpriteHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Image image && data.Data is ImageSpriteData spriteData)
            {
                image.sprite = spriteData.Sprite;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Image) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            if (stateData.Data == null)
            {
                stateData.Data = ScriptableObject.CreateInstance<ImageSpriteData>();
            }
            
            var spriteData = stateData.Data as ImageSpriteData;
            EditorGUI.BeginChangeCheck();
            spriteData.Sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", spriteData.Sprite, typeof(Sprite), false);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }

    /// <summary>
    /// Image 컴포넌트의 Color를 변경하는 핸들러
    /// </summary>
    public class ImageColorHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Image image && data.Data is ImageColorData colorData)
            {
                image.color = colorData.Color;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Image) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            if (stateData.Data == null)
            {
                stateData.Data = ScriptableObject.CreateInstance<ImageColorData>();
            }
            
            var colorData = stateData.Data as ImageColorData;
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
