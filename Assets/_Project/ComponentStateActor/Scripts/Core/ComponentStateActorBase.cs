using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace ComponentStateActor
{
    public abstract class ComponentStateActorBase : MonoBehaviour
    {
        [SerializeField] protected Component target;
        [SerializeField] protected SerializedDictionary<string, ComponentStateData> stateDataDict;
        
        protected string currentStateKey = string.Empty;

        public IReadOnlyList<string> StateKeys => stateDataDict.Keys.ToArray();
        
        public void ChangeState(string key)
        {
            if (target == null)
            {
                Debug.LogError("StateImageActor: Target이 설정되지 않았습니다.");
                return;
            }

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("StateImageActor: 빈 키가 전달되었습니다.");
                return;
            }

            if (!stateDataDict.TryGetValue(key, out var stateImageData))
            {
                Debug.LogError($"StateImageActor: '{key}' 키에 해당하는 상태 데이터를 찾을 수 없습니다.");
                return;
            }
            
            ApplyStateData(stateImageData);
            currentStateKey = key;
        }

        protected abstract void Setup();
        protected abstract void ApplyStateData(ComponentStateData stateImageData);
        
        public bool HasState(string key) => stateDataDict.ContainsKey(key);
        public bool HasAsset(string key) => stateDataDict.ContainsKey(key) && stateDataDict[key].HasAsset;
        public bool HasColor(string key) => stateDataDict.ContainsKey(key) && stateDataDict[key].HasColor;
        public ComponentStateData GetStateData(string key) => stateDataDict.ContainsKey(key) ? stateDataDict[key] : ComponentStateData.Empty;
        
        public void AddState(string key, ComponentStateData data)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("StateImageActor: 빈 키가 전달되었습니다.");
                return;
            }
            
            stateDataDict[key] = data;
        }

        public void RemoveState(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("StateImageActor: 빈 키가 전달되었습니다.");
                return;
            }

            stateDataDict.Remove(key);
        }
    }
}