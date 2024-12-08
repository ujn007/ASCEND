using DG.Tweening;
using Game.Events;
using Unity.Cinemachine;
using UnityEngine;

public class SkillCamera : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO _gameEventChannel;

    private CinemachineBasicMultiChannelPerlin _perlin;
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        _gameEventChannel.AddListener<CameraPerlin>(HandleCameraPerlinEvent);
        _gameEventChannel.AddListener<CameraImpulse>(HandleCameraImpulseEvent);
    }

    private void OnDestroy()
    {
        _gameEventChannel.RemoveListener<CameraPerlin>(HandleCameraPerlinEvent);
        _gameEventChannel.RemoveListener<CameraImpulse>(HandleCameraImpulseEvent);
    }

    private void HandleCameraPerlinEvent(CameraPerlin perlin)
    {
        DOTween.To(() => _perlin.AmplitudeGain, x => _perlin.AmplitudeGain = x, perlin.strength, perlin.increaseDur);
    }

    private void HandleCameraImpulseEvent(CameraImpulse impulse)
    {
        _impulseSource.GenerateImpulse(impulse.strength);
    }
}
