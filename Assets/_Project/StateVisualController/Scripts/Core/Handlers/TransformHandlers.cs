using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== Transform / RectTransform 공통: localPosition ====================
    public class LocalPositionHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data
        {
            public Vector3 localPosition;
        }

        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent == null) return;
            var json = SerializeData(data);
            if (string.IsNullOrEmpty(json)) return;
            var parsed = JsonUtility.FromJson<Data>(json);

            if (targetComponent is RectTransform rectTf)
            {
                rectTf.localPosition = parsed.localPosition;
            }
            else if (targetComponent is Transform tf)
            {
                tf.localPosition = parsed.localPosition;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Transform), typeof(RectTransform) };

        public override string SerializeData(StateHandlerData stateData)
        {
            // Editor에서 채운 임시 컨테이너를 직렬화하는 형태가 아니므로,
            // stateData.TextData(JSON) 또는 ColorData 등을 재활용하지 않고
            // Json 직렬화를 위해 Editor 필드로 유지 → 여기서는 stateData.TextData를 사용
            // 규약: stateData.TextData에 JSON 저장
            return stateData.TextData;
        }

        public override void DeserializeData(string jsonData, StateHandlerData stateData)
        {
            stateData.TextData = jsonData ?? string.Empty;
        }

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            var data = string.IsNullOrEmpty(stateData.TextData)
                ? new Data { localPosition = Vector3.zero }
                : JsonUtility.FromJson<Data>(stateData.TextData);

            EditorGUI.BeginChangeCheck();
            var v = EditorGUILayout.Vector3Field("Local Position", data.localPosition);
            if (EditorGUI.EndChangeCheck())
            {
                data.localPosition = v;
                stateData.TextData = JsonUtility.ToJson(data);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }

    // ==================== Transform / RectTransform 공통: localRotation ====================
    public class LocalRotationHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data
        {
            public Vector3 localEulerAngles;
        }

        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent == null) return;
            var json = SerializeData(data);
            if (string.IsNullOrEmpty(json)) return;
            var parsed = JsonUtility.FromJson<Data>(json);

            if (targetComponent is RectTransform rectTf)
            {
                rectTf.localEulerAngles = parsed.localEulerAngles;
            }
            else if (targetComponent is Transform tf)
            {
                tf.localEulerAngles = parsed.localEulerAngles;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Transform), typeof(RectTransform) };

        public override string SerializeData(StateHandlerData stateData)
        {
			return stateData.TextData;
        }

        public override void DeserializeData(string jsonData, StateHandlerData stateData)
        {
            stateData.TextData = jsonData ?? string.Empty;
        }

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            var data = string.IsNullOrEmpty(stateData.TextData)
                ? new Data { localEulerAngles = Vector3.zero }
                : JsonUtility.FromJson<Data>(stateData.TextData);

            EditorGUI.BeginChangeCheck();
            var v = EditorGUILayout.Vector3Field("Local Rotation (Euler)", data.localEulerAngles);
            if (EditorGUI.EndChangeCheck())
            {
                data.localEulerAngles = v;
                stateData.TextData = JsonUtility.ToJson(data);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }

    // ==================== Transform / RectTransform 공통: localScale ====================
    public class LocalScaleHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data
        {
            public Vector3 localScale;
        }

        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent == null) return;
            var json = SerializeData(data);
            if (string.IsNullOrEmpty(json)) return;
            var parsed = JsonUtility.FromJson<Data>(json);

            if (targetComponent is RectTransform rectTf)
            {
                rectTf.localScale = parsed.localScale;
            }
            else if (targetComponent is Transform tf)
            {
                tf.localScale = parsed.localScale;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Transform), typeof(RectTransform) };

        public override string SerializeData(StateHandlerData stateData)
        {
            return stateData.TextData;
        }

        public override void DeserializeData(string jsonData, StateHandlerData stateData)
        {
            stateData.TextData = jsonData ?? string.Empty;
        }

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            var data = string.IsNullOrEmpty(stateData.TextData)
                ? new Data { localScale = Vector3.zero }
                : JsonUtility.FromJson<Data>(stateData.TextData);

            EditorGUI.BeginChangeCheck();
            var v = EditorGUILayout.Vector3Field("Local Scale", data.localScale);
            if (EditorGUI.EndChangeCheck())
            {
                data.localScale = v;
                stateData.TextData = JsonUtility.ToJson(data);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}


