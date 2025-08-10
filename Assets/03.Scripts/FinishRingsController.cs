using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using System;
using UnityEngine;

public class FinishRingsController : MonoBehaviour
{
    private static readonly int OuterRadius = Shader.PropertyToID("_OuterRadius");
    private static readonly int InnerRadius = Shader.PropertyToID("_InnerRadius");
    
    [SerializeField]
    private MeshRenderer darkRing1Renderer;
    [SerializeField]
    private MeshRenderer lightRing1Renderer;
    [SerializeField]
    private MeshRenderer lightRing2Renderer;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        darkRing1Renderer.material = new Material(darkRing1Renderer.material);
        lightRing1Renderer.material = new Material(lightRing1Renderer.material);
        lightRing2Renderer.material = new Material(lightRing2Renderer.material);
        
        ResetForReplay();
    }
    
    public void ResetForReplay()
    {
        darkRing1Renderer.material.SetFloat(OuterRadius, 0f);
        darkRing1Renderer.material.SetFloat(InnerRadius, 0f);
        
        lightRing1Renderer.material.SetFloat(OuterRadius, 0f);
        lightRing1Renderer.material.SetFloat(InnerRadius, 0f);
        
        lightRing2Renderer.material.SetFloat(OuterRadius, 0f);
        lightRing2Renderer.material.SetFloat(InnerRadius, 0f);
    }

    [Button]
    public void TestPlay()
    {
        ResetForReplay();
        Play().Forget();
    }

    public async UniTask Play()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        
        // 어두운 링이 화면을 덮을만큼 커짐
        darkRing1Renderer.material.DOFloat(0.3f, OuterRadius, 0.3f);
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        // 밝은 링1이 커짐
        await lightRing1Renderer.material.DOFloat(0.19f, OuterRadius, 0.4f).SetEase(Ease.OutCubic);
        // 어두운 링 끄기
        darkRing1Renderer.material.SetFloat(InnerRadius, 0.3f);
        // 밝은 링2가 커짐
        lightRing2Renderer.material.DOFloat(0.2f, OuterRadius, 0.3f).SetEase(Ease.OutQuint);
        // 밝은 링2가 화면 끝까지 커지며 사라짐
        lightRing1Renderer.material.DOFloat(0.3f, OuterRadius, 0.3f);
        lightRing1Renderer.material.DOFloat(0.3f, InnerRadius, 0.3f).SetEase(Ease.OutCubic);
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        // 밝은 링1이 얇아지며 사라짐
        await lightRing2Renderer.material.DOFloat(0.2f, InnerRadius, 0.8f).SetEase(Ease.OutCubic);
        
        ProductionManager.Instance.OnFinishRingsAnimFinish();
    }
}
