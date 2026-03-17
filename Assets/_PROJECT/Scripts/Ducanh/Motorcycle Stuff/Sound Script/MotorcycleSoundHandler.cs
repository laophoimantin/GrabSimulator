using System.Collections;
using UnityEngine;


public class MotorcycleSoundHandler : MonoBehaviour
{

    [Header("Engine Audio Sources")]
    [SerializeField] private AudioSource engineToggleAudioSource;
    [SerializeField] private AudioSource engineRunAudioSource;


    [Header("Gameplay Audio Sources")]
    [SerializeField] private AudioSource driftAudioSource;
    [SerializeField] private AudioSource collisionAudioSource;


    [Header("Sound of what motorcycle?")]
    [SerializeField] private MotorcycleSoundType motorcycleSoundType;


    // Main
    private MotorcycleSound motorcycleSound;


    // Engine Sound References
    private AudioClip engineStartAudio;
    private AudioClip engineRunAudio;


    // Key Turn Sound References
    private AudioClip keyturnAudio;


    // Drifting Sound References
    private AudioClip driftAudio;
    private AudioClip collisionAudio;


    // Engine Start Volume
    private float maxEngineStartVolume;


    // Engine Run Volume
    private float maxEngineVolume;


    // Drift Volume
    private float minDriftVolume;
    private float maxDriftVolume;


    // Collision Volume
    private float minCollisionVolume;
    private float maxCollisionVolume;


    // Collision Pitch
    private float minCollisionPitch;
    private float maxCollisionPitch;


    // Sound Variable
    private float minPitch;
    private float maxPitch;
    private float timeTillEngineRun;


    // Variable
    private bool assigned = false;
    private bool engineEngaged = false;
    private bool fadeEngine = false;


    // Engine Coroutines
    private Coroutine startEngineCoroutine = null; // for engine start
    private Coroutine fadeEngineCoroutine = null; // for fading in/out engine sound


    // Drifting Coroutine
    private Coroutine driftCoroutine = null; // for fading in/out drifting sound



    private void Start()
    {
        StartCoroutine(Initialization());
    }


    private IEnumerator Initialization()
    {
        if (SoundManager.Instance.MotorcycleSoundList.Length == 0)
        {
            Debug.LogWarning("No motorcycle sound is assigned!");
            yield break;
        }


        foreach (MotorcycleSound sound in SoundManager.Instance.MotorcycleSoundList)
        {
            if (motorcycleSoundType.ToString() == sound.name)
            {
                motorcycleSound = sound;
                assigned = true;

                break;
            }

            yield return null;
        }


        if (assigned)
        {
            engineStartAudio = motorcycleSound.EngineStart;
            engineRunAudio = motorcycleSound.EngineRun;  

            minPitch = motorcycleSound.MinPitch;
            maxPitch = motorcycleSound.MaxPitch;
            timeTillEngineRun = motorcycleSound.TimeTillEngineRun;

            maxEngineStartVolume = motorcycleSound.MaxEngineStartVolume;
            maxEngineVolume = motorcycleSound.MaxEngineVolume;


            minDriftVolume = SoundManager.Instance.MotorcycleVolumeStats.MinDriftVolume;
            maxDriftVolume = SoundManager.Instance.MotorcycleVolumeStats.MaxDriftVolume;

            minCollisionVolume = SoundManager.Instance.MotorcycleVolumeStats.MinCollisionVolume;
            maxCollisionVolume = SoundManager.Instance.MotorcycleVolumeStats.MaxCollisionVolume;

            minCollisionPitch = SoundManager.Instance.MotorcycleVolumeStats.MinCollisionPitch;
            maxCollisionPitch = SoundManager.Instance.MotorcycleVolumeStats.MaxCollisionPitch;
        }


        driftAudio = SoundManager.Instance.GetRandomClip(SoundType.Drift_Sound);
        if (driftAudio == null) Debug.Log("Assign Drift_Sound in SoundManager");
        else
        {
            driftAudioSource.clip = driftAudio;
            driftAudioSource.loop = true;
            driftAudioSource.volume = 0f;
        }


        collisionAudio = SoundManager.Instance.GetRandomClip(SoundType.Collision_Sound);
        if (collisionAudio == null) Debug.Log("Assign Collision_Sound in SoundManager");
        else
        {
            collisionAudioSource.clip = collisionAudio;
            collisionAudioSource.loop = false;
            collisionAudioSource.volume = 0f;
        }

        keyturnAudio = SoundManager.Instance.GetRandomClip(SoundType.Key_Turn_Sound);
        if (keyturnAudio == null) Debug.Log("Assign Key_Turn_Sound in SoundManager");
    }



    #region ENGINE START AUDIO
    public void StartEngineSound() // Once the player starts moving, this plays.
    {
        if (startEngineCoroutine != null)
        {
            StopCoroutine(startEngineCoroutine);
            startEngineCoroutine = null;
        }

        startEngineCoroutine = StartCoroutine(StartingEngineSound());
    }
    private IEnumerator StartingEngineSound()
    {
        if (engineRunAudioSource == null)
        {
            Debug.LogWarning("No available audio source");
            yield break;
        }

        EngineAudio(engineStartAudio, isStartEngine: true);
        yield return new WaitForSeconds(timeTillEngineRun);

        EngineAudio(engineRunAudio, isStartEngine: false);
        FadeEngineSound(FadeOption.FadeIn);

        startEngineCoroutine = null;
    }
    #endregion



    #region ENGINE RUN SOUND
    public void EngineSound(float velocityOffset, bool isGrounded)
    {
        if (engineRunAudioSource.clip == null) return;

        float clampedVelosityOffset = Mathf.Max(0f, velocityOffset);
        float speedMain = Mathf.Lerp(minPitch, maxPitch, clampedVelosityOffset);

        engineRunAudioSource.pitch = speedMain;

        float fadeSpeed = 3f;
        float targetVolume = isGrounded ? maxEngineVolume : maxEngineVolume / 2f;

        if (!Mathf.Approximately(engineRunAudioSource.volume, targetVolume))
        {
            float tempVolume = Mathf.MoveTowards(engineRunAudioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
            engineRunAudioSource.volume = tempVolume;
        }
        else
        {
            engineRunAudioSource.volume = targetVolume;
        }
    }
    #endregion



    #region DRIFT SOUND
    public void DriftSound(float velocityMagnitude, float maxSpeed, bool isDrifting)
    {
        if (driftAudioSource.clip == null) return;

        if (!driftAudioSource.isPlaying)
        {
            driftAudioSource.Play();
        }

        float normalizeSpeed = Mathf.Clamp01(velocityMagnitude / maxSpeed);
        float curvedPercentage = SoundManager.Instance.MotorcycleVolumeStats.DriftVolumeCurve.Evaluate(normalizeSpeed);

        float targetVolume = Mathf.Lerp(minDriftVolume, maxDriftVolume, curvedPercentage);

        if (!isDrifting)
        {
            targetVolume = 0f;
        }

        float fadeSpeed = 3f;
        driftAudioSource.volume = Mathf.MoveTowards(driftAudioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
    }
    #endregion



    #region COLLISION SOUND
    public void CollisionSound(float impactSpeed, float maxSpeed)
    {
        if (collisionAudioSource == null) return;

        float crashSeverityPercentage = Mathf.Clamp01(impactSpeed / maxSpeed);
        float targetVolume = Mathf.Lerp(minCollisionVolume, maxCollisionVolume, crashSeverityPercentage);
        float targetPitch = Mathf.Lerp(maxCollisionPitch, minCollisionPitch, crashSeverityPercentage);

        collisionAudioSource.pitch = targetPitch;
        collisionAudioSource.volume = targetVolume;
        collisionAudioSource.PlayOneShot(collisionAudio);
    }
    #endregion



    #region Helpers & Extra Functionalities
    //-------------------------------------

    private void EngineAudio(AudioClip engineAudio, bool isStartEngine)
    {
        if (engineAudio == null) return;

        if (isStartEngine)
        {
            engineToggleAudioSource.PlayOneShot(engineAudio, maxEngineStartVolume);
        }
        else
        {
            engineRunAudioSource.loop = true;
            engineRunAudioSource.clip = engineAudio;
            engineRunAudioSource.Play();
        }
    }


    private void FadeEngineSound(FadeOption option)
    {
        if (fadeEngineCoroutine != null)
        {
            StopCoroutine(fadeEngineCoroutine);
            fadeEngineCoroutine = null;
        }

        fadeEngineCoroutine = StartCoroutine(FadingEngineSound(option));
    }
    private IEnumerator FadingEngineSound(FadeOption option)
    {
        float currentTime = 0f;
        float fadeDuration = 0.25f;

        bool isFadeOut = option == FadeOption.FadeOut ? true : false;

        float targetVolume = option == FadeOption.FadeIn ? maxEngineVolume : 0f;

        if (option == FadeOption.FadeIn)
        {
            engineRunAudioSource.volume = 0f;
        }

        float startVolume = engineRunAudioSource.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float percent = currentTime / fadeDuration;

            engineRunAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, percent);

            yield return null;
        }

        engineRunAudioSource.volume = targetVolume;

        if (isFadeOut) engineRunAudioSource.Pause();

        fadeEngineCoroutine = null;
    }


    public void DisengageEngineSound() // If players stand still for too long, turn engine off.
    {
        FadeEngineSound(FadeOption.FadeOut);
        engineToggleAudioSource.PlayOneShot(keyturnAudio, SoundManager.Instance.MotorcycleVolumeStats.KeyturnVolume);
    }

    //--------
    #endregion

}

