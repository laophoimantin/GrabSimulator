using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(AudioSource))]
public class AmbienceZone : MonoBehaviour
{
    [Header("Zone Identification")]
    [Tooltip("Higher number = Higher priority. Eg: 10 > 1.")]
    [SerializeField] private int zonePriority = 1;

    [Header("Audio Settings")]
    [SerializeField] private AmbienceSoundType ambientSound;
    [SerializeField] private float ambienceVolume = 0.5f;
    [SerializeField] private float fadeDuration = 1.5f;

    // Internal State
    private AudioSource zoneAudioSource;
    private Coroutine ambienceFadeCoroutine;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;

        zoneAudioSource = GetComponent<AudioSource>();
        zoneAudioSource.loop = true;
        zoneAudioSource.volume = 0f;
        zoneAudioSource.playOnAwake = false;
        zoneAudioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        AudioClip clipToPlay = SoundManager.Instance.GetAmbienceClip(ambientSound);
        if (clipToPlay != null) zoneAudioSource.clip = clipToPlay;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            SoundManager.Instance.RegisterZone(this, zonePriority);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            SoundManager.Instance.UnregisterZone(this);
    }

    // Called by the SoundManager Conductor
    public void SetZoneState(bool isBoss)
    {
        float targetVolume = isBoss ? ambienceVolume : 0f;

        if (ambienceFadeCoroutine != null) StopCoroutine(ambienceFadeCoroutine);
        ambienceFadeCoroutine = StartCoroutine(FadeAmbienceRoutine(targetVolume));
    }

    private IEnumerator FadeAmbienceRoutine(float targetVolume)
    {
        if (targetVolume > 0f && !zoneAudioSource.isPlaying) zoneAudioSource.Play();

        float startVol = zoneAudioSource.volume;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            zoneAudioSource.volume = Mathf.Lerp(startVol, targetVolume, timeElapsed / fadeDuration);
            yield return null;
        }

        zoneAudioSource.volume = targetVolume;
        if (targetVolume == 0f) zoneAudioSource.Pause();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.2f); // Blue for Ambience
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }
}