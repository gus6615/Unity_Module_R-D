using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
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

    /// <summary>
    /// 상태 변경을 처리하는 핸들러의 베이스 클래스
    /// 각 컴포넌트 타입별로 구체적인 핸들러가 이를 상속받아 구현
    /// MonoBehaviour를 상속받지 않으므로 Target 오브젝트에 컴포넌트로 부착되지 않음
    /// </summary>
    public abstract class BaseStateHandler
    {
        protected Component targetComponent;
        
        /// <summary>
        /// 상태 데이터를 적용하는 추상 메서드
        /// </summary>
        /// <param name="data">적용할 상태 데이터</param>
        public abstract void ApplyState(StateHandlerData data);
        
        /// <summary>
        /// 이 핸들러가 처리할 수 있는 컴포넌트 타입들을 반환
        /// </summary>
        /// <returns>대상 컴포넌트 타입 배열</returns>
        public abstract Type[] GetTargetComponentType();
        
        /// <summary>
        /// 타겟 컴포넌트를 설정
        /// </summary>
        /// <param name="component">설정할 컴포넌트</param>
        public virtual void SetTargetComponent(Component component)
        {
            targetComponent = component;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 에디터에서 상태 데이터 필드를 그리는 가상 메서드
        /// 각 핸들러에서 오버라이드하여 구현
        /// </summary>
        /// <param name="stateData">상태 데이터</param>
        /// <param name="controller">컨트롤러 참조</param>
        public virtual void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            EditorGUILayout.HelpBox($"No field drawer implemented for handler type: {GetType().Name}", MessageType.Warning);
        }
#endif
    }
}
