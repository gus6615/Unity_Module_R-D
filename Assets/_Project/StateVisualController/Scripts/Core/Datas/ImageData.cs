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
        
        private void OnValidate()
        {
            if (sprite == null)
            {
                Debug.LogWarning($"ImageSpriteData '{name}' has no sprite assigned!");
            }
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
        
        private void OnValidate()
        {
            // Color 값이 유효한 범위 내에 있는지 확인
            color.r = Mathf.Clamp01(color.r);
            color.g = Mathf.Clamp01(color.g);
            color.b = Mathf.Clamp01(color.b);
            color.a = Mathf.Clamp01(color.a);
        }
    }
}
