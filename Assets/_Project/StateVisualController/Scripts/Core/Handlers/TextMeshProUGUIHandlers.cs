using System;
using UnityEngine;
using TMPro;

namespace StateVisualController
{
    // ==================== TMPText 관련 핸들러들 ====================
    
    /// <summary>
    /// TMPText 컴포넌트의 내용을 변경하는 핸들러
    /// </summary>
    public class TextMeshProUGUIContentHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is TextMeshProUGUI text && data.Data is TextMeshProUGUIContentData textData)
            {
                text.text = textData.Content;
            }
        }

        public override Type[] GetTargetComponentType() => new Type[] { typeof(TextMeshProUGUI) };
    }
}
