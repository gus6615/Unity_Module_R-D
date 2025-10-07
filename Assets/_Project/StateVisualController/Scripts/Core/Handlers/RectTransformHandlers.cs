using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== RectTransform 전용: anchoredPosition ====================
    public class AnchoredPositionHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data
        {
            public Vector2 anchoredPosition;
        }

        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is not RectTransform rectTf) return;
            var json = SerializeData(data);
            if (string.IsNullOrEmpty(json)) return;
            var parsed = JsonUtility.FromJson<Data>(json);
            rectTf.anchoredPosition = parsed.anchoredPosition;
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(RectTransform) };

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
                ? new Data { anchoredPosition = Vector2.zero }
                : JsonUtility.FromJson<Data>(stateData.TextData);

            EditorGUI.BeginChangeCheck();
            var v = EditorGUILayout.Vector2Field("Anchored Position", data.anchoredPosition);
            if (EditorGUI.EndChangeCheck())
            {
                data.anchoredPosition = v;
                stateData.TextData = JsonUtility.ToJson(data);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }

    // ==================== RectTransform 전용: size(=width,height via sizeDelta) ====================
    public class SizeDeltaHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data
        {
            public float width;
            public float height;
        }

        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is not RectTransform rectTf) return;
            var json = SerializeData(data);
            if (string.IsNullOrEmpty(json)) return;
            var parsed = JsonUtility.FromJson<Data>(json);
            rectTf.sizeDelta = new Vector2(parsed.width, parsed.height);
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(RectTransform) };

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
                ? new Data { width = 100f, height = 100f }
                : JsonUtility.FromJson<Data>(stateData.TextData);

            EditorGUI.BeginChangeCheck();
            var w = EditorGUILayout.FloatField("Width", data.width);
            var h = EditorGUILayout.FloatField("Height", data.height);
            if (EditorGUI.EndChangeCheck())
            {
                data.width = w;
                data.height = h;
                stateData.TextData = JsonUtility.ToJson(data);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }

    // ==================== RectTransform 전용: offsetMin/offsetMax ====================
    public class OffsetMinMaxHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data
        {
            public Vector2 offsetMin; // left, bottom
            public Vector2 offsetMax; // right, top
        }

        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is not RectTransform rectTf) return;
            var json = SerializeData(data);
            if (string.IsNullOrEmpty(json)) return;
            var parsed = JsonUtility.FromJson<Data>(json);
            rectTf.offsetMin = parsed.offsetMin;
            rectTf.offsetMax = parsed.offsetMax;
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(RectTransform) };

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
                ? new Data { offsetMin = Vector2.zero, offsetMax = Vector2.zero }
                : JsonUtility.FromJson<Data>(stateData.TextData);

            EditorGUI.BeginChangeCheck();
            var min = EditorGUILayout.Vector2Field("Offset Min (L,B)", data.offsetMin);
            var max = EditorGUILayout.Vector2Field("Offset Max (R,T)", data.offsetMax);
            if (EditorGUI.EndChangeCheck())
            {
                data.offsetMin = min;
                data.offsetMax = max;
                stateData.TextData = JsonUtility.ToJson(data);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}


