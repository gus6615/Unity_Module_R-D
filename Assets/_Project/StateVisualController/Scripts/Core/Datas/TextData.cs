using UnityEngine;

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
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(content))
            {
                Debug.LogWarning($"TextContentData '{name}' has empty content!");
            }
        }
    }
}
