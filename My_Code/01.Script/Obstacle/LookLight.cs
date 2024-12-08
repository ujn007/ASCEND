  using UnityEngine;

public class LookLight : MonoBehaviour
{
    [SerializeField] private Transform targetPoint; 

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(targetPoint.position - transform.position);
    }
}
