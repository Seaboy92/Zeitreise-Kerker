
using UnityEngine;

public class PickupAudioPlayer : MonoBehaviour
{
    public static PickupAudioPlayer Instance { get; private set; }

    private AudioSource audioSource;

    private void Awake() {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPickupSound() {
        Debug.Log("PlayPickupSound wurde aufgerufen.");

        if (audioSource == null || audioSource.clip == null) {
            Debug.LogError("AudioSource oder AudioClip fehlt!");
            return;
        }

        audioSource.PlayOneShot(audioSource.clip);
    }
}
