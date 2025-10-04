using System;
using UnityEngine;

namespace StateVisualController
{
    // ==================== GameObject 관련 핸들러들 ====================
    
    /// <summary>
    /// GameObject의 활성화 상태를 변경하는 핸들러
    /// </summary>
    public class GameObjectActiveHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Transform && data.Data is GameObjectActiveData activeData)
            {
                targetComponent.gameObject.SetActive(activeData.IsActive);
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(Transform), typeof(RectTransform) };
    }
}
