using System;
using UnityEngine;
using UnityEngine.UI;

namespace StateVisualController
{
    // ==================== Text 관련 핸들러들 ====================
    
    /// <summary>
    /// Text 컴포넌트의 내용을 변경하는 핸들러
    /// </summary>
    public class TextContentHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Text text && data.Data is TextContentData textData)
            {
                text.text = textData.Content;
            }
        }

        public override Type GetTargetComponentType() => typeof(Text);
    }
}
