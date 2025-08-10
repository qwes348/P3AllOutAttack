using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.Playables;

public class ProductionManager : MonoBehaviour
{
    private static ProductionManager instance;
    public static ProductionManager Instance => instance;

    [SerializeField]
    private PlayableDirector finishCamTimeline;
    [SerializeField]
    private GameObject ppVolumeObject;

    private UnityChanHero unityChan;
    private VictoryCanvas victoryCanvas;
    private FinishRingsController finishRingsController;
    private Vector3 finishCamOriginPos;
    private Vector3 finishCamOriginRot;

    private void Awake()
    {
        if(instance == null)
            instance = this;

        unityChan = FindAnyObjectByType<UnityChanHero>();
        victoryCanvas = FindAnyObjectByType<VictoryCanvas>(FindObjectsInactive.Include);
        finishRingsController = FindAnyObjectByType<FinishRingsController>();
        finishCamOriginPos = finishCamTimeline.transform.position;
        finishCamOriginRot = finishCamTimeline.transform.eulerAngles;
    }

    private void Start()
    {
        Replay();
    }

    public void OnUnityChanFinishAnimFinish()
    {
        // finishRingsController.Play().Forget();
    }

    public void OnFinishRingsAnimFinish()
    {
        finishCamTimeline.Play();
        unityChan.PlayVictoryAnimation();
    }

    public void OnUnityChanVictoryAnimFinish()
    {
        victoryCanvas.Init();
        victoryCanvas.gameObject.SetActive(true);
        victoryCanvas.PlayAnimation().Forget();
        
        unityChan.gameObject.SetActive(false);
        ppVolumeObject.SetActive(false);
    }

    [Button]
    public void Replay()
    {
        victoryCanvas.gameObject.SetActive(false);
        unityChan.gameObject.SetActive(true);
        ppVolumeObject.SetActive(true);
        unityChan.PlayFinishAnimation();
        finishRingsController.ResetForReplay();
        finishRingsController.Play().Forget();
        // 피니쉬캠 타임라인 0프레임 위치로 리셋
        finishCamTimeline.transform.position = finishCamOriginPos;
        finishCamTimeline.transform.eulerAngles = finishCamOriginRot;
    }
}
