using PJH.Combat;
using System.Collections.Generic;
using FMODUnity;
using PJH.Manager;
using UnityEngine;
using static PJH.Core.MInterface;


public class ArrowRain : MonoBehaviour, IPoolable
{
    [field: SerializeField] public PoolTypeSO PoolType { get; set; }
    private PoolManagerSO poolManager;
    [SerializeField] private int power;
    [SerializeField] private EventReference _impactEventReference;
    private IDamageable damageObj = null;

    private ParticleSystem particle;
    private List<ParticleSystem.Particle> _enterParticles = new();
    private Collider[] _colliders;

    public GameObject GameObject => gameObject;

    private void Awake()
    {
        poolManager = Managers.Addressable.Load<PoolManagerSO>("PoolManager");

        particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (EndParticle())
        {
            poolManager.Push(this);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out IDamageable health))
        {
            CombatData combatData = new CombatData
            {
                damage = power,
                hitPoint = other.transform.position
            };
            RuntimeManager.PlayOneShot(_impactEventReference, other.transform.position);
            health.ApplyDamage(combatData);
        }
    }

    private bool EndParticle() => particle.isStopped;

    public void SetUpPool(Pool pool)
    {
    }

    public void ResetItem()
    {
    }
}