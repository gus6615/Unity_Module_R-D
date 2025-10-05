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
            if (targetComponent is Image image)
            {
                image.sprite = data.SpriteData;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Image) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            EditorGUI.BeginChangeCheck();
            Sprite newSprite = stateData.SpriteData;
            newSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", newSprite, typeof(Sprite), false);
            
            if (EditorGUI.EndChangeCheck())
            {
                stateData.SpriteData = newSprite;
                stateData.HandlerType = GetType().Name;
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
            if (targetComponent is Image image)
            {
                image.color = data.ColorData;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Image) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            EditorGUI.BeginChangeCheck();
            Color newColor = stateData.ColorData;
            newColor = EditorGUILayout.ColorField("Color", newColor);
            
            if (EditorGUI.EndChangeCheck())
            {
                stateData.ColorData = newColor;
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
