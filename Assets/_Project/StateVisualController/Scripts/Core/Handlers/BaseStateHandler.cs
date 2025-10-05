using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    /// <summary>
    /// 각 상태별 핸들러 데이터를 저장하는 클래스
    /// ScriptableObject 대신 직접 직렬화 가능한 데이터 구조 사용
    /// </summary>
    [Serializable]
    public class StateHandlerData
    {
        [SerializeField] private string stateName;
        [SerializeField] private string handlerType;
        [SerializeField] private string serializedData; // JSON 형태로 직렬화된 데이터
        
        public string StateName 
        { 
            get => stateName; 
            set => stateName = value; 
        }
        
        public string HandlerType 
        { 
            get => handlerType; 
            set => handlerType = value; 
        }
        
        public string SerializedData 
        { 
            get => serializedData; 
            set => serializedData = value; 
        }
        
        public StateHandlerData()
        {
            stateName = string.Empty;
            handlerType = string.Empty;
            serializedData = string.Empty;
        }
        
        public StateHandlerData(string stateName)
        {
            this.stateName = stateName;
            this.handlerType = string.Empty;
            this.serializedData = string.Empty;
        }
        
        public StateHandlerData(string stateName, string handlerType)
        {
            this.stateName = stateName;
            this.handlerType = handlerType;
            this.serializedData = string.Empty;
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

        /// <summary>
        /// 핸들러 데이터를 JSON 문자열로 직렬화
        /// </summary>
        /// <param name="stateData">직렬화할 상태 데이터</param>
        /// <returns>JSON 문자열</returns>
        public virtual string SerializeData(StateHandlerData stateData)
        {
            return string.Empty; // 기본 구현은 빈 문자열 반환
        }

        /// <summary>
        /// JSON 문자열에서 핸들러 데이터를 역직렬화
        /// </summary>
        /// <param name="jsonData">역직렬화할 JSON 문자열</param>
        /// <param name="stateData">데이터를 저장할 상태 데이터 객체</param>
        public virtual void DeserializeData(string jsonData, StateHandlerData stateData)
        {
            // 기본 구현은 아무것도 하지 않음
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
