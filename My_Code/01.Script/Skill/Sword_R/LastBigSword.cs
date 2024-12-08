using PJH.Combat;
using PJH.Core;
using UnityEngine;

public class LastBigSword : MonoBehaviour, IPoolable
{
    [field: SerializeField] public PoolTypeSO PoolType { get; set; }
    [SerializeField] private MeshRenderer swordDissolver;
    [SerializeField] private float knockBackPower;
    [SerializeField] private float radius;
    [SerializeField] private int skillPower;

    private Collider[] detectColliders;

    public GameObject GameObject => gameObject;

    private void Awake()
    {
        detectColliders = new Collider[10];
    }

    public void DectectEnemy()
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
                    Vector3 dir = (detectColliders[i].transform.position - transform.position);
                    dir.y = 0;

                    CombatData combatData = new()
                    {
                        damageCategory = Define.EDamageCategory.Normal,
                        damage = skillPower,
                        knockBackDir = dir,
                        knockBackDuration = 0.2f,
                        knockBackPower = this.knockBackPower,
                        hitPoint = hitPoint
                    };

                    health.ApplyDamage(combatData);
                }
            }
        }
    }

    public void ResetItem()
    {
        swordDissolver.sharedMaterial.SetFloat("_DissolveAmount", 0);
    }

    public void SetUpPool(Pool pool)
    {
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
