using TMPro;
using UnityEngine;

namespace ComponentStateActor
{
	public sealed class TMPTextStateActor : ComponentStateActorBase
	{
		private TMP_Text targetText;

		protected override void Setup()
		{
			if (target is TMP_Text text)
			{
				targetText = text;
			}
			else
			{
				Debug.LogError("TMPTextStateActor: Target이 TMP_Text 컴포넌트가 아닙니다.");
			}
		}

		protected override void ApplyStateData(ComponentStateData stateData)
		{
			if (targetText == null)
			{
				Debug.LogError("TMPTextStateActor: Target TMP_Text가 설정되지 않았습니다.");
				return;
			}

			// Asset 해석 규칙:
			// - TextAsset: text 내용을 설정
			// - TMP_FontAsset: 폰트 변경
			if (stateData.asset is TextAsset textAsset)
			{
				targetText.text = textAsset.text;
			}
			else if (stateData.asset is TMP_FontAsset fontAsset)
			{
				targetText.font = fontAsset;
			}
			else if (stateData.asset != null)
			{
				Debug.LogError("TMPTextStateActor: 상태 데이터의 Asset 타입이 지원되지 않습니다. (지원: TextAsset, TMP_FontAsset)");
			}

			// 색상 적용
			targetText.color = stateData.color;
		}
	}
}


