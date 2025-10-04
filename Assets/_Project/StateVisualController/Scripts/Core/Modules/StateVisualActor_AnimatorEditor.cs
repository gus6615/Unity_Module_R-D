using UnityEngine;

namespace StateVisualController
{
	/// <summary>
	/// Animator(Mecanim) 제어용 State Actor
	/// asset로 RuntimeAnimatorController 또는 AnimationClip을 받아서 적용/재생합니다.
	/// color는 사용하지 않습니다.
	/// </summary>
	public sealed class StateVisualActor_AnimatorEditor : StateVisualControllerBase
	{
		private Animator targetAnimator;

		protected override void Setup()
		{
			if (target is Animator anim)
			{
				targetAnimator = anim;
			}
			else
			{
				Debug.LogError("AnimatorStateActor: Target이 Animator 컴포넌트가 아닙니다.");
			}
		}

		protected override void ApplyStateData(StateVisualData stateVisualData)
		{
			if (targetAnimator == null)
			{
				Debug.LogError("AnimatorStateActor: Target Animator가 설정되지 않았습니다.");
				return;
			}

			if (stateVisualData.asset is AnimationClip clip)
			{
				var baseController = targetAnimator.runtimeAnimatorController;
				if (baseController == null)
				{
					Debug.LogError("AnimatorStateActor: Animator Controller가 없습니다. Controller를 먼저 설정하세요.");
					return;
				}

				var clipName = clip.name;
				targetAnimator.Play(clipName, 0, 0f);
			}
			else
			{
				Debug.LogError("AnimatorStateActor: 상태 데이터의 Asset 타입이 지원되지 않습니다. (지원: RuntimeAnimatorController, AnimationClip)");
			}
		}
	}
}


