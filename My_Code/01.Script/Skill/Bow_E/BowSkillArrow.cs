using UnityEngine;

public class BowSkillArrow : MonoBehaviour, IPoolable
{
    [SerializeField] private float force;

    [field: SerializeField] public PoolTypeSO PoolType { get; set; }

    public GameObject GameObject => gameObject;
    protected Pool _myPool;

    public void ShotUp(Vector3 targetPos, float angle)
    {
        Vector3 dir = targetPos - transform.position;
        Quaternion shotAngle = Quaternion.LookRotation(dir);

        Quaternion rot = Quaternion.Euler(-angle, shotAngle.eulerAngles.y, transform.rotation.z);
        transform.rotation = rot;
    }

    public virtual void ShotUp(Quaternion targetPos)
    {
        transform.rotation = targetPos;
    }

    protected virtual void Update()
    {
        transform.position += transform.forward * force * Time.deltaTime;
    }

    public void ResetTrm(Transform _player)
    {
        transform.rotation = Quaternion.Euler(-80, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.position = _player.transform.position;
    }

    public virtual void SetUpPool(Pool pool)
    {
        _myPool = pool;
    }

    public virtual void ResetItem()
    {
    }


    //public Vector3 startPos;
    //public Vector3 targetPos;
    //public float height = 5f;
    //public float duration = 2f;

    //private Vector3 randPosition;
    //private Rigidbody rigid;
    //private Vector3 previousPosition;

    //private float timeElapsed;

    //private void Awake()
    //{
    //    rigid = GetComponent<Rigidbody>();
    //}

    //void Start()
    //{
    //    startPos = transform.position;
    //    timeElapsed = 0f;

    //    SetRandomPointInCircle();
    //}

    //void Update()
    //{
    //    ShotArrow();
    //}

    //private void ShotArrow()
    //{
    //    if (timeElapsed < duration)
    //    {
    //        timeElapsed += Time.deltaTime;
    //        float t = timeElapsed / duration;

    //        previousPosition = transform.position;

    //        Vector3 currentPosition = CalculateParabolicPosition(startPos, targetPos, height, t);
    //        transform.position = currentPosition;

    //        RotateArrowTowardsTarget(currentPosition);
    //    }
    //}

    //private Vector3 currentPosition;
    //private Vector3 CalculateParabolicPosition(Vector3 start, Vector3 end, float height, float t)
    //{
    //    Vector3 horizontalPosition = Vector3.Lerp(start, end, t);
    //    float parabola = 4 * height * t * (1 - t);
    //    currentPosition = new Vector3(horizontalPosition.x, horizontalPosition.y + parabola, horizontalPosition.z);
    //    return currentPosition;
    //}

    //private void RotateArrowTowardsTarget(Vector3 currentPosition)
    //{
    //    Vector3 direction = currentPosition - previousPosition;

    //    if (direction != Vector3.zero)
    //    {
    //        transform.rotation = Quaternion.LookRotation(direction);
    //    }
    //}

    //public void SetRandomPointInCircle()
    //{
    //    float angle = Random.Range(0f, Mathf.PI * 2);
    //    float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * 3;
    //    float x = distance * Mathf.Cos(angle);
    //    float z = distance * Mathf.Sin(angle);
    //    targetPos = new Vector3(targetPos.x + x, targetPos.y, targetPos.z + z);
    //}


    //public void SetTargetPos(Vector3 pos) => targetPos = pos;
}