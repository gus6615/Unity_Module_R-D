using UnityEngine;

namespace StateVisualController
{
    // ==================== TextMeshProUGUI 관련 데이터 클래스들 ====================
    
    /// <summary>
    /// TextMeshProUGUI 내용 변경을 위한 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "TextMeshProUGUIContentData", menuName = "StateVisualController/TextMeshProUGUI Content Data")]
    public class TextMeshProUGUIContentData : ScriptableObject
    {
        [SerializeField] private string content = string.Empty;
        
        public string Content 
        { 
            get => content; 
            set => content = value ?? string.Empty; 
        }
    }
}
