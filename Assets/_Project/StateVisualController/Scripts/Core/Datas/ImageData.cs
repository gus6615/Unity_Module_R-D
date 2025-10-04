using UnityEngine;

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
}
