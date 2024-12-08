using System.Collections;
using UnityEngine;

public class LightLookPoint : MonoBehaviour
{
    [SerializeField] private Transform clampPoint1, clampPoint2;
    [SerializeField] private float nearDistance;
    [SerializeField] private float pointSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float drag;

    private Vector3 moveForce;
    private Vector3 previousPos;
    private float pointX, pointZ;

    private bool startMove = true;
    private bool isNear;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(MovingLookPoint());
    }

    private void Update()
    {
        Movement();
        CheckNear();
    }

    private void CheckNear()
    {
        float dis = Vector3.Distance(transform.position, previousPos);
        isNear = dis <= nearDistance ? true : false;
    }

    private void Movement()
    {
        Vector3 targetDir = previousPos;
        moveForce += (targetDir - transform.position) * pointSpeed * Time.deltaTime;
        moveForce = Vector3.ClampMagnitude(moveForce, maxSpeed);
        rigid.linearVelocity = new Vector3(moveForce.x, rigid.linearVelocity.y, moveForce.z);

        moveForce *= drag;
    }

    private IEnumerator MovingLookPoint()
    {
        while (startMove)
        {
            GetRandomPosition();

            yield return new WaitUntil(() => isNear);
        }
    }

    private void GetRandomPosition()
    {
        Vector3 newPos;
        float distance;

        do
        {
            float pointX = Random.Range(clampPoint1.position.x, clampPoint2.position.x);
            float pointZ = Random.Range(clampPoint1.position.z, clampPoint2.position.z);

            newPos = new Vector3(pointX, clampPoint1.position.y, pointZ);
            distance = Vector3.Distance(previousPos, newPos);

        } while (distance < Mathf.Abs(clampPoint1.position.x / 2));
 
        previousPos = newPos;
    }
}
