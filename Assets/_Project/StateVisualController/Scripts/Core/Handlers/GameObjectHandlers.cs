using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== GameObject 관련 데이터 클래스들 ====================
    
    /// <summary>
    /// GameObject 활성화 상태 변경을 위한 직렬화 가능한 데이터 클래스
    /// </summary>
    [Serializable]
    public class GameObjectActiveData
    {
        public bool isActive;
        
        public GameObjectActiveData()
        {
            isActive = true;
        }
        
        public GameObjectActiveData(bool isActive)
        {
            this.isActive = isActive;
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
            if (targetComponent is Transform && !string.IsNullOrEmpty(data.SerializedData))
            {
                try
                {
                    var activeData = JsonUtility.FromJson<GameObjectActiveData>(data.SerializedData);
                    targetComponent.gameObject.SetActive(activeData.isActive);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to apply gameobject active state: {e.Message}");
                }
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Transform), typeof(RectTransform) };

        public override string SerializeData(StateHandlerData stateData)
        {
            if (targetComponent is Transform transform)
            {
                var activeData = new GameObjectActiveData(transform.gameObject.activeSelf);
                return JsonUtility.ToJson(activeData);
            }
            return string.Empty;
        }

        public override void DeserializeData(string jsonData, StateHandlerData stateData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    var activeData = JsonUtility.FromJson<GameObjectActiveData>(jsonData);
                    // 데이터가 유효한지 확인
                    if (activeData != null)
                    {
                        stateData.SerializedData = jsonData;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to deserialize gameobject active data: {e.Message}");
                }
            }
        }

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            GameObjectActiveData activeData = null;
            
            // 기존 데이터가 있으면 로드
            if (!string.IsNullOrEmpty(stateData.SerializedData))
            {
                try
                {
                    activeData = JsonUtility.FromJson<GameObjectActiveData>(stateData.SerializedData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load gameobject active data: {e.Message}");
                }
            }
            
            // 데이터가 없으면 새로 생성
            if (activeData == null)
            {
                activeData = new GameObjectActiveData();
            }
            
            EditorGUI.BeginChangeCheck();
            bool newActive = activeData.isActive;
            newActive = EditorGUILayout.Toggle("Active", newActive);
            
            if (EditorGUI.EndChangeCheck())
            {
                var newActiveData = new GameObjectActiveData(newActive);
                stateData.SerializedData = JsonUtility.ToJson(newActiveData);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
