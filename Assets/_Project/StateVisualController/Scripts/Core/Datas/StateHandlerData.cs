using System;
using UnityEngine;

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
}
