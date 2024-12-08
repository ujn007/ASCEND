using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MoveTwoPos
{
    public Vector3 startPos;
    public Vector3 endPos;
}

public class SwordPlayerClone : MonoBehaviour, IPoolable
{
    [TabGroup("Info")][SerializeField] private float moveDuration;
    [TabGroup("Info")][SerializeField] private AnimationCurve moveEase;

    [SerializeField] private TrailRenderer trail;

    [field: SerializeField] public PoolTypeSO PoolType { get; set; }
    public GameObject GameObject => gameObject;

    private Pool _pool;

    public void MoveToWhere(MoveTwoPos moveTwoPos)
    {
        transform.position = moveTwoPos.startPos;

        transform.LookAt(moveTwoPos.endPos);

        transform.DOMove(moveTwoPos.endPos, moveDuration).SetEase(moveEase)
            .OnComplete(() =>
            {
                trail.emitting = false;
                _pool.Push(this);
            });
    }

    public void SetUpPool(Pool pool)
    {
        _pool = pool;
    }

    public void ResetItem()
    {
        trail.emitting = true;
    }
}
