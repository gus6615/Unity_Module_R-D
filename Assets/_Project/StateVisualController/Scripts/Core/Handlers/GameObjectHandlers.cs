using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    // ==================== GameObject 관련 핸들러들 ====================
    
    /// <summary>
    /// GameObject의 활성화 상태를 변경하는 핸들러
    /// </summary>
    public class GameObjectActiveHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Transform && data.Data is GameObjectActiveData activeData)
            {
                targetComponent.gameObject.SetActive(activeData.IsActive);
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Transform), typeof(RectTransform) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            if (stateData.Data == null)
            {
                stateData.Data = ScriptableObject.CreateInstance<GameObjectActiveData>();
            }
            
            var activeData = stateData.Data as GameObjectActiveData;
            EditorGUI.BeginChangeCheck();
            activeData.IsActive = EditorGUILayout.Toggle("Active", activeData.IsActive);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
