using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== Image 관련 핸들러들 ====================
    
    /// <summary>
    /// Image 컴포넌트의 Sprite를 변경하는 핸들러
    /// </summary>
    public class ImageSpriteHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data { public string spriteGuid; }
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Image image)
            {
                var json = data.TextData;
                if (!string.IsNullOrEmpty(json))
                {
                    var parsed = JsonUtility.FromJson<Data>(json);
#if UNITY_EDITOR
                    if (!string.IsNullOrEmpty(parsed.spriteGuid))
                    {
                        var path = AssetDatabase.GUIDToAssetPath(parsed.spriteGuid);
                        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                        image.sprite = sprite;
                    }
                    else
                    {
                        image.sprite = null;
                    }
#else
                    // 런타임에서는 GUID 로딩이 불가하므로 sprite 적용 생략
#endif
                }
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Image) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            Sprite current = null;
            if (!string.IsNullOrEmpty(stateData.TextData))
            {
                var parsed = JsonUtility.FromJson<Data>(stateData.TextData);
                if (!string.IsNullOrEmpty(parsed.spriteGuid))
                {
                    var path = AssetDatabase.GUIDToAssetPath(parsed.spriteGuid);
                    current = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }
            }

            EditorGUI.BeginChangeCheck();
            Sprite newSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", current, typeof(Sprite), false);
            if (EditorGUI.EndChangeCheck())
            {
                var newData = new Data
                {
                    spriteGuid = newSprite != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newSprite)) : string.Empty
                };
                stateData.TextData = JsonUtility.ToJson(newData);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }

    /// <summary>
    /// Image 컴포넌트의 Color를 변경하는 핸들러
    /// </summary>
    public class ImageColorHandler : BaseStateHandler
    {
        [Serializable]
        private struct Data { public Color color; }
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Image image)
            {
                var json = data.TextData;
                var parsed = string.IsNullOrEmpty(json)
                    ? new Data { color = Color.white }
                    : JsonUtility.FromJson<Data>(json);
                image.color = parsed.color;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Image) };

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            var data = string.IsNullOrEmpty(stateData.TextData)
                ? new Data { color = Color.white }
                : JsonUtility.FromJson<Data>(stateData.TextData);

            EditorGUI.BeginChangeCheck();
            var newColor = EditorGUILayout.ColorField("Color", data.color);
            if (EditorGUI.EndChangeCheck())
            {
                data.color = newColor;
                stateData.TextData = JsonUtility.ToJson(data);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
