using DG.Tweening;
using FMODUnity;
using Game.Events;
using UnityEngine;

public class ChargeEffect : MonoBehaviour, IPoolable
{
    [field: SerializeField] public PoolTypeSO PoolType { get; set; }
    [SerializeField] private GameEventChannelSO eventSO;
    [SerializeField] private float scaleUpDuration;
    [SerializeField] private EventReference _chargeingReference;
    public GameObject GameObject => gameObject;
    private Pool _pool;

    public void ScaleUp()
    {
        RuntimeManager.PlayOneShot(_chargeingReference, transform.position);
        var evt = GameEvents.CameraPerlin;
        evt.strength = 2; evt.increaseDur = scaleUpDuration;
        eventSO.RaiseEvent(evt);

        transform.DOScale(4, scaleUpDuration).OnComplete(() => _pool.Push(this));
    }

    public void ResetItem()
    {
        transform.localScale = Vector3.zero;
    }

    public void SetUpPool(Pool pool)
    {
        _pool = pool;
    }
}
