
using UnityEngine;
using UnityEngine.UI;

namespace ComponentStateActor
{
    public sealed class ImageStateActor : ComponentStateActorBase
    {
        private Image targetImage;
        
        protected override void Setup()
        {
            if (target is Image image)
            {
                targetImage = image;
            }
            else
            {
                Debug.LogError("StateImageActor: Target이 Image 컴포넌트가 아닙니다.");
            }
        }

        protected override void ApplyStateData(ComponentStateData stateData)
        {
            if (targetImage == null)
            {
                Debug.LogError("StateImageActor: Target Image가 설정되지 않았습니다.");
                return;
            }
            
            if (stateData.asset is Sprite sprite)
            {
                targetImage.sprite = sprite;
            }
            else
            {
                Debug.LogError("StateImageActor: 상태 데이터의 Asset이 Sprite 타입이 아닙니다.");
            }

            targetImage.color = stateData.color;
        }
    }
}