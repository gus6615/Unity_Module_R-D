using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StateVisualController
{
    /// <summary>
    /// 컴포넌트 타입별로 사용 가능한 핸들러들을 관리하는 레지스트리
    /// 런타임에서 동적으로 핸들러를 등록하고 조회할 수 있음
    /// </summary>
    public static class HandlerRegistry
    {
        private static Dictionary<Type, List<Type>> componentToHandlers;
        private static bool isInitialized = false;

        /// <summary>
        /// 레지스트리 초기화 (지연 초기화)
        /// </summary>
        private static void Initialize()
        {
            if (isInitialized) return;
            
            componentToHandlers = new Dictionary<Type, List<Type>>();
            RegisterDefaultHandlers();
            isInitialized = true;
        }

        /// <summary>
        /// 기본 핸들러들을 등록
        /// </summary>
        private static void RegisterDefaultHandlers()
        {
            // Image 컴포넌트용 핸들러들
            RegisterHandler(typeof(Image), typeof(ImageSpriteHandler));
            RegisterHandler(typeof(Image), typeof(ImageColorHandler));

            // Text 컴포넌트용 핸들러들
            RegisterHandler(typeof(Text), typeof(TextContentHandler));

            // Transform(GameObject) 컴포넌트용 핸들러들
            RegisterHandler(typeof(Transform), typeof(GameObjectActiveHandler));
            RegisterHandler(typeof(RectTransform), typeof(GameObjectActiveHandler));
        }

        /// <summary>
        /// 특정 컴포넌트 타입에 핸들러를 등록
        /// </summary>
        /// <param name="componentType">컴포넌트 타입</param>
        /// <param name="handlerType">핸들러 타입</param>
        private static void RegisterHandler(Type componentType, Type handlerType)
        {
            if (!componentToHandlers.ContainsKey(componentType))
            {
                componentToHandlers[componentType] = new List<Type>();
            }
            
            if (!componentToHandlers[componentType].Contains(handlerType))
            {
                componentToHandlers[componentType].Add(handlerType);
            }
        }

        /// <summary>
        /// 특정 컴포넌트 타입에 사용 가능한 핸들러 목록을 반환
        /// </summary>
        /// <param name="componentType">컴포넌트 타입</param>
        /// <returns>사용 가능한 핸들러 타입 목록</returns>
        public static List<Type> GetHandlersForComponent(Type componentType)
        {
            Initialize();
            
            return componentToHandlers.ContainsKey(componentType) 
                ? new List<Type>(componentToHandlers[componentType]) // 복사본 반환
                : new List<Type>();
        }

        /// <summary>
        /// 커스텀 핸들러를 동적으로 등록
        /// </summary>
        /// <param name="componentType">컴포넌트 타입</param>
        /// <param name="handlerType">핸들러 타입</param>
        /// <returns>등록 성공 여부</returns>
        public static bool RegisterCustomHandler(Type componentType, Type handlerType)
        {
            Initialize();
            
            if (!typeof(BaseStateHandler).IsAssignableFrom(handlerType))
            {
                Debug.LogError($"{handlerType.Name} must inherit from BaseStateHandler");
                return false;
            }

            if (componentType == null)
            {
                Debug.LogError("Component type cannot be null");
                return false;
            }

            RegisterHandler(componentType, handlerType);
            Debug.Log($"Successfully registered custom handler {handlerType.Name} for component {componentType.Name}");
            return true;
        }

        /// <summary>
        /// 특정 컴포넌트 타입에서 핸들러를 제거
        /// </summary>
        /// <param name="componentType">컴포넌트 타입</param>
        /// <param name="handlerType">제거할 핸들러 타입</param>
        /// <returns>제거 성공 여부</returns>
        public static bool UnregisterHandler(Type componentType, Type handlerType)
        {
            Initialize();
            
            if (!componentToHandlers.ContainsKey(componentType))
            {
                Debug.LogWarning($"No handlers registered for component type {componentType.Name}");
                return false;
            }

            bool removed = componentToHandlers[componentType].Remove(handlerType);
            if (removed)
            {
                Debug.Log($"Successfully unregistered handler {handlerType.Name} from component {componentType.Name}");
            }
            else
            {
                Debug.LogWarning($"Handler {handlerType.Name} was not registered for component {componentType.Name}");
            }
            
            return removed;
        }

        /// <summary>
        /// 핸들러 타입 이름으로 핸들러 인스턴스를 생성
        /// </summary>
        /// <param name="handlerTypeName">핸들러 타입 이름</param>
        /// <returns>생성된 핸들러 인스턴스</returns>
        public static BaseStateHandler CreateHandler(string handlerTypeName)
        {
            if (string.IsNullOrEmpty(handlerTypeName))
                return null;
                
            try
            {
                // 핸들러 타입 이름으로 Type 찾기
                Type handlerType = Type.GetType($"StateVisualController.{handlerTypeName}");
                if (handlerType == null)
                {
                    Debug.LogError($"Handler type '{handlerTypeName}' not found");
                    return null;
                }
                
                // 핸들러 인스턴스 생성 (컴포넌트로 추가하지 않음)
                var handler = Activator.CreateInstance(handlerType) as BaseStateHandler;
                return handler;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create handler '{handlerTypeName}': {e.Message}");
                return null;
            }
        }

    }
}
