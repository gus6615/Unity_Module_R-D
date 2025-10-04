using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateSystem
{
    // ==================== 핵심 데이터 구조 ====================
    
    /// <summary>
    /// 상태 정보를 저장하는 데이터 클래스
    /// </summary>
    [Serializable]
    public class StateData
    {
        [SerializeField] private string stateName;
        [SerializeField] private bool isFoldout = true;
        
        public string StateName 
        { 
            get => stateName; 
            set => stateName = value; 
        }
        
        public bool IsFoldout 
        { 
            get => isFoldout; 
            set => isFoldout = value; 
        }
        
        public StateData(string name)
        {
            stateName = name;
            isFoldout = true;
        }
        
        public StateData()
        {
            stateName = string.Empty;
            isFoldout = true;
        }
    }

    /// <summary>
    /// 액터(대상 오브젝트) 정보를 저장하는 데이터 클래스
    /// </summary>
    [Serializable]
    public class ActorData
    {
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private GameObject target;
        [SerializeField] private Component selectedComponent;
        [SerializeField] private string selectedComponentType;
        [SerializeField] private BaseStateHandler handler;
        [SerializeField] private string handlerType;
        [SerializeField] private List<StateHandlerData> stateDataList = new List<StateHandlerData>();
        
        public bool IsEnabled 
        { 
            get => isEnabled; 
            set => isEnabled = value; 
        }
        
        public GameObject Target 
        { 
            get => target; 
            set => target = value; 
        }
        
        public Component SelectedComponent 
        { 
            get => selectedComponent; 
            set => selectedComponent = value; 
        }
        
        public string SelectedComponentType 
        { 
            get => selectedComponentType; 
            set => selectedComponentType = value; 
        }
        
        public BaseStateHandler Handler 
        { 
            get => handler; 
            set => handler = value; 
        }
        
        public string HandlerType 
        { 
            get => handlerType; 
            set => handlerType = value; 
        }
        
        public List<StateHandlerData> StateDataList 
        { 
            get => stateDataList; 
            set => stateDataList = value; 
        }
        
        public ActorData()
        {
            isEnabled = true;
            stateDataList = new List<StateHandlerData>();
        }
    }

    // ==================== 메인 컨트롤러 ====================

    /// <summary>
    /// 상태 시각화를 관리하는 메인 컨트롤러
    /// 다양한 상태에 따라 UI 요소들의 시각적 표현을 제어
    /// </summary>
    public class StateVisualController : MonoBehaviour
    {
        [SerializeField] private List<StateData> states = new List<StateData>();
        [SerializeField] private List<ActorData> actors = new List<ActorData>();
        [SerializeField] private string currentState = string.Empty;

        // Properties
        public List<StateData> States => states;
        public List<ActorData> Actors => actors;
        public string CurrentState => currentState;

        #region State Management

        /// <summary>
        /// 새로운 상태를 추가
        /// </summary>
        /// <param name="stateName">추가할 상태 이름</param>
        /// <returns>추가 성공 여부</returns>
        public bool AddState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                Debug.LogWarning("State name cannot be null or empty!");
                return false;
            }

            if (states.Exists(s => s.StateName == stateName))
            {
                Debug.LogWarning($"State '{stateName}' already exists!");
                return false;
            }

            states.Add(new StateData(stateName));

            // 모든 Actor에 새로운 상태 데이터 추가
            foreach (var actor in actors)
            {
                if (actor.Handler != null)
                {
                    actor.StateDataList.Add(new StateHandlerData(stateName));
                }
            }

            Debug.Log($"Successfully added state: {stateName}");
            return true;
        }

        /// <summary>
        /// 상태를 제거
        /// </summary>
        /// <param name="stateName">제거할 상태 이름</param>
        /// <returns>제거 성공 여부</returns>
        public bool RemoveState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
            {
                Debug.LogWarning("State name cannot be null or empty!");
                return false;
            }

            int removedCount = states.RemoveAll(s => s.StateName == stateName);

            if (removedCount == 0)
            {
                Debug.LogWarning($"State '{stateName}' not found!");
                return false;
            }

            // 모든 Actor에서 해당 상태 데이터 제거
            foreach (var actor in actors)
            {
                actor.StateDataList.RemoveAll(d => d.StateName == stateName);
            }

            // 현재 상태가 제거된 상태라면 초기화
            if (currentState == stateName)
            {
                currentState = string.Empty;
            }

            Debug.Log($"Successfully removed state: {stateName}");
            return true;
        }

        /// <summary>
        /// 현재 상태를 설정하고 적용
        /// </summary>
        /// <param name="stateName">설정할 상태 이름</param>
        /// <returns>설정 성공 여부</returns>
        public bool SetState(string stateName)
        {
            // 빈 문자열인 경우 현재 상태를 초기화
            if (string.IsNullOrEmpty(stateName))
            {
                currentState = string.Empty;
                Debug.Log("Current state cleared");
                return true;
            }

            if (!states.Exists(s => s.StateName == stateName))
            {
                Debug.LogWarning($"State '{stateName}' does not exist!");
                return false;
            }

            currentState = stateName;
            ApplyState();
            Debug.Log($"Current state set to: {stateName}");
            return true;
        }

        #endregion

        #region Actor Management

        /// <summary>
        /// 새로운 액터를 추가
        /// </summary>
        /// <returns>추가된 액터의 인덱스</returns>
        public int AddActor()
        {
            actors.Add(new ActorData());
            Debug.Log($"Added new actor at index: {actors.Count - 1}");
            return actors.Count - 1;
        }

        /// <summary>
        /// 액터를 제거
        /// </summary>
        /// <param name="index">제거할 액터의 인덱스</param>
        /// <returns>제거 성공 여부</returns>
        public bool RemoveActor(int index)
        {
            if (index < 0 || index >= actors.Count)
            {
                Debug.LogWarning($"Actor index {index} is out of range!");
                return false;
            }

            var actor = actors[index];
            if (actor.Handler != null && actor.Target != null)
            {
                DestroyImmediate(actor.Handler);
            }

            actors.RemoveAt(index);
            Debug.Log($"Successfully removed actor at index: {index}");
            return true;
        }

        #endregion

        #region Handler Management

        /// <summary>
        /// 액터의 타겟이 변경되었을 때 호출
        /// </summary>
        /// <param name="actor">변경된 액터</param>
        public void OnActorTargetChanged(ActorData actor)
        {
            if (actor == null) return;

            // 기존 핸들러 제거
            if (actor.Handler != null && actor.Target != null)
            {
                DestroyImmediate(actor.Handler);
            }

            // 타겟 변경 시 관련 데이터 초기화
            actor.SelectedComponent = null;
            actor.SelectedComponentType = null;
            actor.Handler = null;
            actor.HandlerType = null;

            Debug.Log($"Actor target changed for: {actor.Target?.name}");
        }

        /// <summary>
        /// 액터의 컴포넌트가 선택되었을 때 호출
        /// </summary>
        /// <param name="actor">액터</param>
        /// <param name="component">선택된 컴포넌트</param>
        public void OnComponentSelected(ActorData actor, Component component)
        {
            if (actor == null) return;

            actor.SelectedComponent = component;
            actor.SelectedComponentType = component?.GetType().Name;

            Debug.Log($"Component selected: {component?.GetType().Name}");
        }

        /// <summary>
        /// 액터의 핸들러가 선택되었을 때 호출
        /// </summary>
        /// <param name="actor">액터</param>
        /// <param name="handlerType">선택된 핸들러 타입</param>
        /// <returns>핸들러 설정 성공 여부</returns>
        public bool OnHandlerSelected(ActorData actor, Type handlerType)
        {
            if (actor == null || actor.Target == null || handlerType == null)
            {
                Debug.LogWarning("Actor, Target, or HandlerType is null!");
                return false;
            }

            // 기존 Handler 제거
            if (actor.Handler != null)
            {
                DestroyImmediate(actor.Handler);
            }

            // 새 Handler 추가
            actor.Handler = actor.Target.AddComponent(handlerType) as BaseStateHandler;
            if (actor.Handler == null)
            {
                Debug.LogError($"Failed to add handler component: {handlerType.Name}");
                return false;
            }

            actor.HandlerType = handlerType.Name;
            actor.Handler.SetTargetComponent(actor.SelectedComponent);

            // 모든 상태에 대한 데이터 초기화
            actor.StateDataList.Clear();
            foreach (var state in states)
            {
                actor.StateDataList.Add(new StateHandlerData(state.StateName));
            }

            Debug.Log($"Handler set: {handlerType.Name}");
            return true;
        }

        #endregion

        #region State Application

        /// <summary>
        /// 현재 상태를 모든 활성화된 액터에 적용
        /// </summary>
        private void ApplyState()
        {
            if (string.IsNullOrEmpty(currentState))
            {
                Debug.LogWarning("No current state set!");
                return;
            }

            int appliedCount = 0;
            foreach (var actor in actors)
            {
                if (!actor.IsEnabled || actor.Handler == null) continue;

                var stateData = actor.StateDataList.Find(d => d.StateName == currentState);
                if (stateData != null)
                {
                    actor.Handler.ApplyState(stateData);
                    appliedCount++;
                }
            }

            Debug.Log($"Applied state '{currentState}' to {appliedCount} actors");
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// 특정 상태가 존재하는지 확인
        /// </summary>
        /// <param name="stateName">확인할 상태 이름</param>
        /// <returns>존재 여부</returns>
        public bool HasState(string stateName)
        {
            return states.Exists(s => s.StateName == stateName);
        }

        /// <summary>
        /// 액터의 개수를 반환
        /// </summary>
        /// <returns>액터 개수</returns>
        public int GetActorCount()
        {
            return actors.Count;
        }

        /// <summary>
        /// 상태의 개수를 반환
        /// </summary>
        /// <returns>상태 개수</returns>
        public int GetStateCount()
        {
            return states.Count;
        }

        #endregion
    }
}