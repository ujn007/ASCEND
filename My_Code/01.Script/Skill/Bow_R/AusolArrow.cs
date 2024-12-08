using DG.Tweening;
using FMODUnity;
using Game.Events;
using INab.Dissolve;
using PJH.Combat;
using PJH.Core;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AusolArrow : MonoBehaviour, IPoolable
{
    [field: SerializeField] public PoolTypeSO PoolType { get; set; }
    [SerializeField] private GameEventChannelSO eventSO;
    [SerializeField] private MeshRenderer dissolverMesh;
    [SerializeField] private Transform collisionTrm;
    [SerializeField] private float speed;
    [SerializeField] private EventReference _shootReference;


    [TabGroup("Info")] [SerializeField] private int skillPower;
    [TabGroup("Info")] [SerializeField] private float radius;
    [TabGroup("Info")] [SerializeField] private Transform castTrm;
    [TabGroup("Info")] [SerializeField] private Vector3 boxSize;

    private Collider[] detectColliders;

    private Pool _pool;

    private Tween moveTween;

    private Dissolver dissolver;

    public GameObject GameObject => gameObject;

    private HashSet<MInterface.IDamageable> detectEnemyHash = new HashSet<MInterface.IDamageable>();

    private void Awake()
    {
        dissolver = GetComponent<Dissolver>();
        detectColliders = new Collider[10];
    }

    public void ShotFront(Vector3 targetPos)
    {
        var evt = GameEvents.CameraPerlin;
        evt.strength = 0; evt.increaseDur = 0.1f;
        eventSO.RaiseEvent(evt);

        RuntimeManager.PlayOneShot(_shootReference, transform.position);
        Impulse();

        Vector3 dir = targetPos - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
        moveTween = transform.DOMove(targetPos + dir * 10, speed)
            .OnUpdate(() => DetectEnemy())
            .SetEase(Ease.Linear)
            .SetSpeedBased()
            .OnComplete(() => _pool.Push(this));
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((Define.MLayerMask.WhatIsWall & (1 << other.gameObject.layer)) != 0)
        {
            Impulse();
            moveTween.Kill();

            Sequence sq = DOTween.Sequence();
            sq.AppendInterval(3);
            sq.AppendCallback(() => dissolver.Dissolve());
            sq.AppendInterval(1).OnComplete(() => _pool.Push(this));
        }
    }

    private void DetectEnemy()
    {
        int cnt = Physics.OverlapBoxNonAlloc(
            castTrm.position, boxSize * 2 , detectColliders, Quaternion.identity,Define.MLayerMask.WhatIsEnemy);

        if (cnt > 0)
        {
            for (int i = 0; i < cnt; i++)
            {
                if (detectColliders[i].TryGetComponent(out MInterface.IDamageable health) )
                {
                    if (detectEnemyHash.Contains(health)) continue;

                    detectEnemyHash.Add(health);
                    Vector3 hitPoint = detectColliders[i].ClosestPointOnBounds(transform.position);

                    CombatData combatData = new()
                    {
                        damageCategory = Define.EDamageCategory.Normal,
                        damage = skillPower,
                        hitPoint = hitPoint
                    };

                    health.ApplyDamage(combatData);
                }
            }
        }
    }

    public void ResetItem()
    {
        dissolverMesh.sharedMaterial.SetFloat("_DissolveAmount", 0);
        detectEnemyHash.Clear();
    }

    public void SetUpPool(Pool pool)
    {
        _pool = pool;
    }

    private void Impulse()
    {
        var evt = GameEvents.CameraImpulse;
        evt.strength = 2;
        eventSO.RaiseEvent(evt);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(castTrm.position, boxSize);
    }
#endif

}
