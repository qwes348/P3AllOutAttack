using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;

public class VictoryCanvas : MonoBehaviour
{
    private static readonly int InnerRadius = Shader.PropertyToID("_InnerRadius");
    private static readonly int OuterRadius = Shader.PropertyToID("_OuterRadius");
    
    [SerializeField]
    private Image unmaskRing;
    [SerializeField]
    private Image unmaskCircle;
    [SerializeField]
    private RectTransform massImage;
    [SerializeField]
    private Image unityChanImage;
    [SerializeField]
    private RectTransform commentTextRect;
    [SerializeField]
    private float standardTime = 1f;    // 기준시간
    
    private Material unmaskRingMat;
    private float originInnerRadius;
    private float originOuterRadius;
    private bool isInitialized = false;

    [Button]
    public void Init()
    {
        // 리플레이했을 때 두번 들어오지않게 처리
        if (isInitialized)
            return;
        
        // 플레이 중에만 변경사항이 유지되도록 인스턴스를 사용
        unmaskRingMat = new Material(unmaskRing.material);
        unmaskRing.material = unmaskRingMat;
        
        originInnerRadius = unmaskRingMat.GetFloat(InnerRadius);
        originOuterRadius = unmaskRingMat.GetFloat(OuterRadius);
        
        isInitialized = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            PlayAnimationTest();
    }

    [Button]
    public void PlayAnimationTest()
    {
        PlayAnimation().Forget();
    }

    public async UniTask PlayAnimation()
    {
        // 초기화
        StopAllCoroutines();
        unmaskRing.transform.localScale = Vector3.zero;
        unmaskRingMat.SetFloat(InnerRadius, originInnerRadius);
        unmaskRingMat.SetFloat(OuterRadius, originOuterRadius);
        unmaskCircle.transform.localScale = Vector3.zero;
        massImage.anchoredPosition = new Vector2(2000f, massImage.anchoredPosition.y);
        commentTextRect.anchoredPosition = new Vector2(commentTextRect.sizeDelta.x - 2100f, commentTextRect.anchoredPosition.y);
        unityChanImage.rectTransform.DOKill();
        unityChanImage.rectTransform.anchoredPosition = new Vector2(-2f, unityChanImage.rectTransform.anchoredPosition.y);
        
        // 링 언마스크 커지는 연출
        unmaskRing.transform.DOScale(5.8f, standardTime);
        // 약간의 딜레이를 줘서 두꺼워지는 연출을 티나게 함
        await UniTask.Delay(TimeSpan.FromSeconds(standardTime * 0.15f));
        // 두꺼워지는 연출
        DOVirtual.Float(unmaskRingMat.GetFloat(OuterRadius), 0.5f, standardTime, v => unmaskRingMat.SetFloat(OuterRadius, v))
            .OnComplete(() =>StartCoroutine(LinearSpeedScaling(unmaskRing.transform, 0.03f)));  // 스케일을 키워서 아주 느리게 내려오는듯한 효과 연출
        // MASS DESTRUCTION 이미지 등장
        massImage.DOAnchorPosX(200f, standardTime * 0.6f).SetEase(Ease.OutQuad);
        // 유니티쨩 이미지 슬금슬금 이동
        unityChanImage.rectTransform.DOAnchorPosX(unityChanImage.rectTransform.anchoredPosition.x - 20f, standardTime *10f);
        
        await UniTask.Delay(TimeSpan.FromSeconds(standardTime * 1.5f));
        // 서클 마스크 등장 및 확대
        unmaskCircle.transform.DOScale(4.3f, standardTime * 1.5f)
            .OnComplete(() => StartCoroutine(LinearSpeedScaling(unmaskCircle.transform, 0.03f)));    // 스케일을 키워서 아주 느리게 내려오는듯한 효과 연출
        await UniTask.Delay(TimeSpan.FromSeconds(standardTime * 0.7f));
        // 링 마스크를 얇게 만들기
        DOVirtual.Float(unmaskRingMat.GetFloat(InnerRadius), 0.485f, standardTime, v => unmaskRingMat.SetFloat(InnerRadius, v));
        // 코멘트 텍스트 등장
        commentTextRect.DOAnchorPosX(-666f, standardTime * 0.2f).SetEase(Ease.OutQuad);
    }

    private IEnumerator LinearSpeedScaling(Transform target, float speed)
    {
        while (true)
        {
            target.localScale += Vector3.one * (speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
