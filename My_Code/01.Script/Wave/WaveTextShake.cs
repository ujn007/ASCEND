using UnityEngine;

public class WaveTextShake : MonoBehaviour
{
    [SerializeField] private float intervalStrong;

    [SerializeField] private float speed = 2.0f;

    private void Update()
    {
        UpdateShake();
    }

    private void UpdateShake()
    {
        float xOffset = Mathf.Sin(Time.time * speed) * intervalStrong * Time.timeScale;
        transform.localPosition = transform.localPosition + (Vector3.right * xOffset);
    }
}