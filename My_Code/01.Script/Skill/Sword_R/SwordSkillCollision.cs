using Game.Events;
using MoreMountains.Feedbacks;
using PJH.Combat;
using PJH.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public class SwordSkillCollision : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO eventSO;

    [TabGroup("Info")][SerializeField] private int skillPower;
    [TabGroup("Info")][SerializeField] private float radius;

    [TabGroup("Clamp")][SerializeField] private Vector3 clampPoint1, clampPoint2;

    [TabGroup("EffectPool")][SerializeField] private PoolTypeSO hitEffectType;

    private Collider[] detectColliders;

    private MMF_Player _hitFeedback;

    private void Awake()
    {
        _hitFeedback = transform.Find("HitFeedback").GetComponent<MMF_Player>();
    }

    public void Initialize(int count, Vector3 mousePos)
    {
        print(count);
        detectColliders = new Collider[10];
        transform.position = mousePos;
        DectectWall();
    }

    private void PlayEffect(PoolTypeSO effectType, Vector3 position, Quaternion rot)
    {
        var evt = SpawnEvents.EffectSpawn;
        evt.effectType = effectType;
        evt.position = position;
        evt.rotation = rot;
        eventSO.RaiseEvent(evt);
    }

    public void DetectEnemy()
    {
        int cnt = Physics.OverlapSphereNonAlloc(
            transform.position, radius, detectColliders, Define.MLayerMask.WhatIsEnemy);

        if (cnt > 0)
        {
            for (int i = 0; i < cnt; i++)
            {
                if (detectColliders[i].TryGetComponent(out MInterface.IDamageable health))
                {
                    Vector3 hitPoint = detectColliders[i].ClosestPointOnBounds(transform.position);

                    CombatData combatData = new()
                    {
                        damageCategory = Define.EDamageCategory.Normal,
                        damage = skillPower,
                        hitPoint = hitPoint
                    };
                    PlayEffect(hitEffectType, hitPoint, Quaternion.Euler(-90, 0, 0));

                    _hitFeedback?.PlayFeedbacks();
                    health.ApplyDamage(combatData);
                }
            }
        }
    }

    private void DectectWall()
    {
        float minX = Mathf.Min(clampPoint1.x, clampPoint2.x);
        float maxX = Mathf.Max(clampPoint1.x, clampPoint2.x);
        float minZ = Mathf.Min(clampPoint1.z, clampPoint2.z);
        float maxZ = Mathf.Max(clampPoint1.z, clampPoint2.z);

        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedZ = Mathf.Clamp(transform.position.z, minZ, maxZ);

        transform.position = new Vector3(clampedX, 18, clampedZ);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
