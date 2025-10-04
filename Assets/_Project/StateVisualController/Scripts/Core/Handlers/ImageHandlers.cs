using System;
using UnityEngine;
using UnityEngine.UI;

namespace StateVisualController
{
    // ==================== Image 관련 핸들러들 ====================
    
    /// <summary>
    /// Image 컴포넌트의 Sprite를 변경하는 핸들러
    /// </summary>
    public class ImageSpriteHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Image image && data.Data is ImageSpriteData spriteData)
            {
                image.sprite = spriteData.Sprite;
            }
        }

        public override Type GetTargetComponentType() => typeof(Image);
    }

    /// <summary>
    /// Image 컴포넌트의 Color를 변경하는 핸들러
    /// </summary>
    public class ImageColorHandler : BaseStateHandler
    {
        public override void ApplyState(StateHandlerData data)
        {
            if (targetComponent is Image image && data.Data is ImageColorData colorData)
            {
                image.color = colorData.Color;
            }
        }

        public override Type GetTargetComponentType() => typeof(Image);
    }
}
