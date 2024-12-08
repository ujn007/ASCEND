using Game.Events;
using FIMSpace.FLook;
using FMODUnity;
using PJH.Manager;
using PJH.Scene;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private SkillitemDealer dealer;
    [SerializeField] private GameEventChannelSO eventSO;
    [SerializeField] private EventReference _visibleSkillItemDealerEventReference;
    private SkillitemDealer _dealerObject;

    private void Awake()
    {
        eventSO.AddListener<GetStoreSkill>(HandleGetSkill);
        eventSO.AddListener<ClearStage>(SpawnDealer);
        _dealerObject = Instantiate(dealer);
        _dealerObject.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        eventSO.RemoveListener<GetStoreSkill>(HandleGetSkill);
        eventSO.RemoveListener<ClearStage>(SpawnDealer);
    }

    private void SpawnDealer(ClearStage evt)
    {
        _dealerObject.gameObject.SetActive(true);
        if (_dealerObject.GetWeaponSkillsCount() <= 0)
            NextWave();

        _dealerObject.GetComponent<FLookAnimator>().ObjectToFollow =
            (Managers.Scene.CurrentScene as GameScene).Player.transform;
        RuntimeManager.PlayOneShot(_visibleSkillItemDealerEventReference, _dealerObject.transform.position);
        _dealerObject.SpawnItemAnimaotion(true);
    }

    private void HandleGetSkill(GetStoreSkill skill)
    {
        NextWave();
    }

    private void NextWave()
    {
        var evt = GameEvents.NextStage;
        eventSO?.RaiseEvent(evt);
        _dealerObject.DestroyMe();
    }
}