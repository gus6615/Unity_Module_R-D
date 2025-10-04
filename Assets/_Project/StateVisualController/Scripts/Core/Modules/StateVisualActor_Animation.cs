using UnityEngine;

namespace StateVisualController
{
	/// <summary>
	/// Legacy Animation 컴포넌트 제어용 State Actor
	/// asset로 AnimationClip을 받아서 재생합니다. color는 사용하지 않습니다.
	/// </summary>
	public sealed class StateVisualActor_Animation : StateVisualControllerBase
	{
		private Animation targetAnimation;

		protected override void Setup()
		{
			if (target is Animation anim)
			{
				targetAnimation = anim;
			}
			else
			{
				Debug.LogError("AnimationStateActor: Target이 Animation 컴포넌트가 아닙니다.");
			}
		}

		protected override void ApplyStateData(StateVisualData stateVisualData)
		{
			if (targetAnimation == null)
			{
				Debug.LogError("AnimationStateActor: Target Animation이 설정되지 않았습니다.");
				return;
			}

			if (stateVisualData.asset is AnimationClip clip)
			{
				targetAnimation.Play(clip.name);
			}
			else
			{
				Debug.LogError("AnimationStateActor: 상태 데이터의 Asset이 AnimationClip 타입이 아닙니다.");
			}
		}
	}
}


