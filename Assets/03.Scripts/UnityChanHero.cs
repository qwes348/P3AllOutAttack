using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class UnityChanHero : MonoBehaviour
{
    private static readonly int animParamVictory = Animator.StringToHash("T_Victory");
    private static readonly int animParamFinish = Animator.StringToHash("T_Finish");
    private static readonly int stateHashVictory = Animator.StringToHash("WIN00");
    private static readonly int stateHashFinish = Animator.StringToHash("Finish");
    
    [SerializeField]
    private float victoryAnimLengthRatio = 0.8f;
    
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayFinishAnimation()
    {
        anim.SetTrigger(animParamFinish);
        WaitAnimationEnd(stateHashFinish, () =>
        {
            ProductionManager.Instance.OnUnityChanFinishAnimFinish();
        }).Forget();
    }

    public void PlayVictoryAnimation()
    {
        anim.SetTrigger(animParamVictory);
        WaitAnimationEnd(stateHashVictory, () => ProductionManager.Instance.OnUnityChanVictoryAnimFinish(), victoryAnimLengthRatio).Forget();
    }

    private async UniTask WaitAnimationEnd(int animHash, Action onAnimEnd, float offset = 1f)
    {
        // 타겟 애니메이션으로 전환될때까지 대기
        await UniTask.WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).shortNameHash == animHash);
        // 타겟 애니메이션이 끝날때까지 대기
        await UniTask.Delay(TimeSpan.FromSeconds(anim.GetCurrentAnimatorStateInfo(0).length) * offset);
        
        onAnimEnd?.Invoke();
    }

    private async UniTask WaitVictoryAnimation()
    {
        // victory 애니메이션으로 전환될때까지 대기
        await UniTask.WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHashVictory);
        // victory 애니메이션이 끝날때까지 대기
        await UniTask.Delay(TimeSpan.FromSeconds(anim.GetCurrentAnimatorStateInfo(0).length * victoryAnimLengthRatio));
        
        ProductionManager.Instance.OnUnityChanVictoryAnimFinish();
    }
}
