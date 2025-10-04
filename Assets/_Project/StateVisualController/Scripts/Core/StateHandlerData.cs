using System;
using UnityEngine;

namespace StateSystem
{
    /// <summary>
    /// 각 상태별 핸들러 데이터를 저장하는 클래스
    /// </summary>
    [Serializable]
    public class StateHandlerData
    {
        [SerializeField] private string stateName;
        [SerializeField] private ScriptableObject data;
        
        public string StateName 
        { 
            get => stateName; 
            set => stateName = value; 
        }
        
        public ScriptableObject Data 
        { 
            get => data; 
            set => data = value; 
        }
        
        public StateHandlerData()
        {
            stateName = string.Empty;
            data = null;
        }
        
        public StateHandlerData(string stateName)
        {
            this.stateName = stateName;
            this.data = null;
        }
    }

    // ==================== Image 관련 데이터 클래스들 ====================
    
    /// <summary>
    /// Image Sprite 변경을 위한 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "ImageSpriteData", menuName = "StateSystem/Image Sprite Data")]
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
    [CreateAssetMenu(fileName = "ImageColorData", menuName = "StateSystem/Image Color Data")]
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

    // ==================== Text 관련 데이터 클래스들 ====================
    
    /// <summary>
    /// Text 내용 변경을 위한 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "TextContentData", menuName = "StateSystem/Text Content Data")]
    public class TextContentData : ScriptableObject
    {
        [SerializeField] private string content = string.Empty;
        
        public string Content 
        { 
            get => content; 
            set => content = value ?? string.Empty; 
        }
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(content))
            {
                Debug.LogWarning($"TextContentData '{name}' has empty content!");
            }
        }
    }

    // ==================== GameObject 관련 데이터 클래스들 ====================
    
    /// <summary>
    /// GameObject 활성화 상태 변경을 위한 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "GameObjectActiveData", menuName = "StateSystem/GameObject Active Data")]
    public class GameObjectActiveData : ScriptableObject
    {
        [SerializeField] private bool isActive = true;
        
        public bool IsActive 
        { 
            get => isActive; 
            set => isActive = value; 
        }
        
        private void OnValidate()
        {
            // Boolean 값은 항상 유효하므로 별도 검증 불필요
        }
    }
}
