using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StateVisualController
{
    // ==================== Image 관련 데이터 클래스들 ====================
    
    /// <summary>
    /// Image Sprite 변경을 위한 직렬화 가능한 데이터 클래스
    /// </summary>
    [Serializable]
    public class ImageSpriteData
    {
        public string spriteGuid; // Sprite의 GUID 저장
        
        public ImageSpriteData()
        {
            spriteGuid = string.Empty;
        }
        
        public ImageSpriteData(Sprite sprite)
        {
            spriteGuid = GetSpriteGuid(sprite);
        }
        
        public Sprite GetSprite()
        {
            if (string.IsNullOrEmpty(spriteGuid))
                return null;
                
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GUIDToAssetPath(spriteGuid);
            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
#else
            // 런타임에서는 Resources 폴더에서 로드하거나 다른 방법 사용
            return Resources.Load<Sprite>(spriteGuid);
#endif
        }
        
        private string GetSpriteGuid(Sprite sprite)
        {
            if (sprite == null)
                return string.Empty;
                
#if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(sprite);
            return AssetDatabase.AssetPathToGUID(assetPath);
#else
            return sprite.name; // 런타임에서는 이름으로 대체
#endif
        }
    }

    /// <summary>
    /// Image Color 변경을 위한 직렬화 가능한 데이터 클래스
    /// </summary>
    [Serializable]
    public class ImageColorData
    {
        public float r, g, b, a;
        
        public ImageColorData()
        {
            r = g = b = a = 1f;
        }
        
        public ImageColorData(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }
        
        public Color GetColor()
        {
            return new Color(r, g, b, a);
        }
    }

    // ==================== Image 관련 핸들러들 ====================
    
    /// <summary>
    /// Image 컴포넌트의 Sprite를 변경하는 핸들러
    /// </summary>
    public class ImageSpriteHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Image image && !string.IsNullOrEmpty(data.SerializedData))
            {
                try
                {
                    var spriteData = JsonUtility.FromJson<ImageSpriteData>(data.SerializedData);
                    image.sprite = spriteData.GetSprite();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to apply sprite state: {e.Message}");
                }
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Image) };

        public override string SerializeData(StateHandlerData stateData)
        {
            if (targetComponent is Image image)
            {
                var spriteData = new ImageSpriteData(image.sprite);
                return JsonUtility.ToJson(spriteData);
            }
            return string.Empty;
        }

        public override void DeserializeData(string jsonData, StateHandlerData stateData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    var spriteData = JsonUtility.FromJson<ImageSpriteData>(jsonData);
                    // 데이터가 유효한지 확인
                    if (spriteData != null)
                    {
                        stateData.SerializedData = jsonData;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to deserialize sprite data: {e.Message}");
                }
            }
        }

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            ImageSpriteData spriteData = null;
            
            // 기존 데이터가 있으면 로드
            if (!string.IsNullOrEmpty(stateData.SerializedData))
            {
                try
                {
                    spriteData = JsonUtility.FromJson<ImageSpriteData>(stateData.SerializedData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load sprite data: {e.Message}");
                }
            }
            
            // 데이터가 없으면 새로 생성
            if (spriteData == null)
            {
                spriteData = new ImageSpriteData();
            }
            
            EditorGUI.BeginChangeCheck();
            Sprite newSprite = spriteData.GetSprite();
            newSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", newSprite, typeof(Sprite), false);
            
            if (EditorGUI.EndChangeCheck())
            {
                var newSpriteData = new ImageSpriteData(newSprite);
                stateData.SerializedData = JsonUtility.ToJson(newSpriteData);
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
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Image image && !string.IsNullOrEmpty(data.SerializedData))
            {
                try
                {
                    var colorData = JsonUtility.FromJson<ImageColorData>(data.SerializedData);
                    image.color = colorData.GetColor();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to apply color state: {e.Message}");
                }
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Image) };

        public override string SerializeData(StateHandlerData stateData)
        {
            if (targetComponent is Image image)
            {
                var colorData = new ImageColorData(image.color);
                return JsonUtility.ToJson(colorData);
            }
            return string.Empty;
        }

        public override void DeserializeData(string jsonData, StateHandlerData stateData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                try
                {
                    var colorData = JsonUtility.FromJson<ImageColorData>(jsonData);
                    // 데이터가 유효한지 확인
                    if (colorData != null)
                    {
                        stateData.SerializedData = jsonData;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to deserialize color data: {e.Message}");
                }
            }
        }

#if UNITY_EDITOR
        public override void DrawFields(StateHandlerData stateData, StateVisualController controller)
        {
            ImageColorData colorData = null;
            
            // 기존 데이터가 있으면 로드
            if (!string.IsNullOrEmpty(stateData.SerializedData))
            {
                try
                {
                    colorData = JsonUtility.FromJson<ImageColorData>(stateData.SerializedData);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load color data: {e.Message}");
                }
            }
            
            // 데이터가 없으면 새로 생성
            if (colorData == null)
            {
                colorData = new ImageColorData();
            }
            
            EditorGUI.BeginChangeCheck();
            Color newColor = colorData.GetColor();
            newColor = EditorGUILayout.ColorField("Color", newColor);
            
            if (EditorGUI.EndChangeCheck())
            {
                var newColorData = new ImageColorData(newColor);
                stateData.SerializedData = JsonUtility.ToJson(newColorData);
                stateData.HandlerType = GetType().Name;
                EditorUtility.SetDirty(controller);
            }
        }
#endif
    }
}
