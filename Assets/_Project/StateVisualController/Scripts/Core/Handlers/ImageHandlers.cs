using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== Image 관련 데이터 클래스들 ====================
    
    /// <summary>
    /// Image Sprite 변경을 위한 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "ImageSpriteData", menuName = "StateVisualController/Image Sprite Data")]
    public class ImageSpriteData : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        
        public Sprite Sprite 
        { 
            get => sprite; 
            set => sprite = value; 
        }
    }

    /// <summary>
    /// Image Color 변경을 위한 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "ImageColorData", menuName = "StateVisualController/Image Color Data")]
    public class ImageColorData : ScriptableObject
    {
        [SerializeField] private Color color = Color.white;
        
        public Color Color 
        { 
            get => color; 
            set => color = value; 
        }
    }

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
