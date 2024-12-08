using UnityEngine;
using RayFire;
using INab.Dissolve;
using System.Threading.Tasks;
using Sirenix.OdinInspector;

public class Mirror : MonoBehaviour, IPoolable
{
    [field: SerializeField] public PoolTypeSO PoolType { get; set; }

    [TabGroup("Dissolve")][SerializeField] private Dissolver glassDisovler;
    [TabGroup("Dissolve")][SerializeField] private Dissolver woodDisovler;

    private MeshRenderer glassMesh, woodMesh;
    private RayfireRigidRoot glassRayFire, woodRayFire;

    public GameObject GameObject => gameObject;

    private RayfireBomb rayfireBomb;

    private Pool _pool;

    private void Awake()
    {
        rayfireBomb = GetComponentInChildren<RayfireBomb>();

        glassMesh = glassDisovler.GetComponentInChildren<MeshRenderer>();
        woodMesh = woodDisovler.GetComponentInChildren<MeshRenderer>();

        glassRayFire = glassDisovler.GetComponent<RayfireRigidRoot>();    
        woodRayFire = woodDisovler.GetComponent<RayfireRigidRoot>();    
    }

    public void Bomb()
    {
        print($"{transform.name}_Bomb");
        rayfireBomb.range = Random.Range(3.7f, 4.3f);
        rayfireBomb.Explode(0);
    }

    public async void Dissolve()
    {
        glassDisovler.Dissolve();
        woodDisovler.Dissolve();

        await Task.Delay(2000);

        ResetRayFire();

        _pool.Push(this);
        print("Ǫ��");
    }

    private void ResetRayFire()
    {
        glassMesh.sharedMaterial.SetFloat("_DissolveAmount", 0);
        woodMesh.sharedMaterial.SetFloat("_DissolveAmount", 0);

        glassRayFire.ResetRigidRoot();
        woodRayFire.ResetRigidRoot();

        glassRayFire.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        woodRayFire.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
     }

    public void ResetItem()
    {
        // rayfireBomb.Restore();
    }

    public void SetUpPool(Pool pool)
    {
        _pool = pool;
    }
}
