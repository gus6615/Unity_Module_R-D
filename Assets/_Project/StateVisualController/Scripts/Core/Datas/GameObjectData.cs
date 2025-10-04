using UnityEngine;

namespace StateVisualController
{
    // ==================== GameObject 관련 데이터 클래스들 ====================
    
    /// <summary>
    /// GameObject 활성화 상태 변경을 위한 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "GameObjectActiveData", menuName = "StateVisualController/GameObject Active Data")]
    public class GameObjectActiveData : ScriptableObject
    {
        [SerializeField] private bool isActive = true;
        
        public bool IsActive 
        { 
            get => isActive; 
            set => isActive = value; 
        }
    }
}
