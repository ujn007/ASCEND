using DG.Tweening;
using System;
using FMODUnity;
using Game.Events;
using PJH.Manager;
using PJH.UI;
using TMPro;
using UnityEngine;

public class WaveManager : LJS.MonoSingleton<WaveManager>
{
    public event Action<int> StartNextStageEvent;
    public event Action<int> BossStageEvent;
    private int currentStage;
    public int CurrentStage => currentStage;

    [Header("StageInfo")] [Header("UI")] [SerializeField]
    private RectTransform parent;

    [SerializeField] private BossStageUI _bossStageUI;
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private float textDuration;
    [SerializeField] private int _finishBossStage = 30;
    [SerializeField] private GameEventChannelSO _stageUpEventChanne;
    [SerializeField] private EventReference _nextStageEventReference;
    private EnemySpawnDataListSO _enemySpawnDataList;
    [Header("Ease")] [SerializeField] private AnimationCurve showEase;

    private void Awake()
    {
        parent.gameObject.SetActive(false);
        _stageUpEventChanne.AddListener<NextStage>(StageUp);
        _enemySpawnDataList = Managers.Addressable.Load<EnemySpawnDataListSO>("EnemySpawnDataList");
    }

    private void OnDestroy()
    {
        _stageUpEventChanne.RemoveListener<NextStage>(StageUp);
    }

    private void StageUp(NextStage evt)
    {
        currentStage++;

        bool isBossStage = BossStage();
        if (!isBossStage)
        {
            RuntimeManager.PlayOneShot(_nextStageEventReference);
            StartNextStageEvent?.Invoke(currentStage);
            UIRender(isBossStage);
        }
        else
        {
            var data = _enemySpawnDataList.spawnDataList[currentStage];
            _bossStageUI.ShowStageUI(data.bossImage, data.bossName);
        }
    }

    public void InitStage()
    {
        currentStage = 0;

        bool isBossStage = BossStage();
        if (!isBossStage)
        {
            RuntimeManager.PlayOneShot(_nextStageEventReference);
            StartNextStageEvent?.Invoke(currentStage);
            UIRender(isBossStage);
        }
        else
        {
            var data = _enemySpawnDataList.spawnDataList[currentStage];
            _bossStageUI.ShowStageUI(data.bossImage, data.bossName);
        }
    }

    private bool BossStage()
    {
        bool isBossStage = _enemySpawnDataList.spawnDataList[currentStage].isBossStage;
        Debug.Log(isBossStage);
        if (isBossStage)
        {
            BossStageEvent?.Invoke(currentStage);
            return true;
        }
        else
            return false;
    }

    private void UIRender(bool isBossStage)
    {
        if (isBossStage)
        {
            stageText.text = _enemySpawnDataList.spawnDataList[currentStage].bossName;
        }
        else
        {
            stageText.text = $"Wave {currentStage}";
        }

        parent.anchoredPosition = Vector2.zero;

        SlideAnimation();
    }

    private void SlideAnimation()
    {
        parent.gameObject.SetActive(true);

        Sequence sq = DOTween.Sequence();
        sq.Append(parent.DOScale(Vector3.one, textDuration).ChangeStartValue(Vector3.zero).SetEase(showEase));
        sq.AppendInterval(1f);
        sq.Append(parent.DOScale(Vector3.one * 1.2f, textDuration / 2).SetEase(Ease.OutCubic)
            .OnComplete(() => parent.DOScale(Vector3.zero, textDuration / 3).OnComplete(() =>
            {
                parent.gameObject.SetActive(false);
            })));
    }

    public int GetCurrentStage() => currentStage;
}