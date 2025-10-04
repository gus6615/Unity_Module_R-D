using System;
using UnityEngine;

namespace StateVisualController
{
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
    }
}
