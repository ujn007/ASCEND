using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class SlayerSound : MonoBehaviour
{
    [TabGroup("SoundEvent")][SerializeField] private EventReference roarSound;
    [TabGroup("SoundEvent")][SerializeField] private EventReference jumpSound;
    [TabGroup("SoundEvent")][SerializeField] private EventReference landingSound;
    [TabGroup("SoundEvent")][SerializeField] private EventReference slashSound;
    [TabGroup("SoundEvent")][SerializeField] private EventReference strongSlashSound;
    [TabGroup("SoundEvent")][SerializeField] private EventReference groundSmashSound;

    private void Raor() => RuntimeManager.PlayOneShot(roarSound, transform.position);
    private void JumpUp () => RuntimeManager.PlayOneShot(jumpSound, transform.position);
    private void Landing () => RuntimeManager.PlayOneShot(landingSound, transform.position);
    private void Slash () => RuntimeManager.PlayOneShot(slashSound, transform.position);
    private void StrongSlash () => RuntimeManager.PlayOneShot(strongSlashSound, transform.position);
    private void GroundSmash() => RuntimeManager.PlayOneShot(groundSmashSound, transform.position);

}
