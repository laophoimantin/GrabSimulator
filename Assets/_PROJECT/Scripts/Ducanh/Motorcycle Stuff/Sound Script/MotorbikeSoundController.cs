using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MotorbikeSoundController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MotorbikePhysics motorPhysics;
    [SerializeField] private MotorbikeInput motorInput;


    [Header("Engine Audio Sources")]
    [SerializeField] private AudioSource engineToggleAudioSource;
    [SerializeField] private AudioSource engineRunAudioSource;


    [Header("Gameplay Audio Sources")]
    [SerializeField] private AudioSource driftAudioSource;
    [SerializeField] private AudioSource collisionAudioSource;


    [Header("Sound of what motorcycle?")]
    [SerializeField] private MotorbikeType motorcycleSoundType;


    // Main
    private MotorbikeSoundSettingList motorcycleSound;


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
    private float timeTillEngineDisengage;
    private float currentDisengageTime;


    // Variable
    private bool assigned = false;
    private bool engineEngaged = false;
    private bool fadeEngine = false;
    private bool engineStarted = false;


    // Engine Coroutines
    private Coroutine startEngineCoroutine = null; // for engine start
    private Coroutine fadeEngineCoroutine = null; // for fading in/out engine sound



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


        if (engineToggleAudioSource != null) engineToggleAudioSource.playOnAwake = false;
        if (engineRunAudioSource != null) engineRunAudioSource.playOnAwake = false;
        if (collisionAudioSource != null) collisionAudioSource.playOnAwake = false;
        if (driftAudioSource != null) driftAudioSource.playOnAwake = false;


        foreach (MotorbikeSoundSettingList sound in SoundManager.Instance.MotorcycleSoundList)
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
            timeTillEngineDisengage = motorcycleSound.TimeTillEngineDisengage;

            maxEngineStartVolume = motorcycleSound.MaxEngineStartVolume;
            maxEngineVolume = motorcycleSound.MaxEngineVolume;


            minDriftVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MinDriftVolume;
            maxDriftVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MaxDriftVolume;

            minCollisionVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MinCollisionVolume;
            maxCollisionVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MaxCollisionVolume;

            minCollisionPitch = SoundManager.Instance.MotorcycleUniversalVolumeStats.MinCollisionPitch;
            maxCollisionPitch = SoundManager.Instance.MotorcycleUniversalVolumeStats.MaxCollisionPitch;
        }


        driftAudio = SoundManager.Instance.GetMotorbikeGameplaySound(MotorbikeGameplaySoundType.Drift_Sound);
        if (driftAudio == null) Debug.Log("Assign Drift_Sound in SoundManager");
        else
        {
            driftAudioSource.clip = driftAudio;
            driftAudioSource.loop = true;
            driftAudioSource.volume = 0f;
        }


        collisionAudio = SoundManager.Instance.GetMotorbikeGameplaySound(MotorbikeGameplaySoundType.Collision_Sound);
        if (collisionAudio == null) Debug.Log("Assign Collision_Sound in SoundManager");
        else
        {
            collisionAudioSource.clip = collisionAudio;
            collisionAudioSource.loop = false;
            collisionAudioSource.volume = 0f;
        }

        keyturnAudio = SoundManager.Instance.GetMotorbikeGameplaySound(MotorbikeGameplaySoundType.Key_Turn_Sound);
        if (keyturnAudio == null) Debug.Log("Assign Key_Turn_Sound in SoundManager");
    }


    private void Update()
    {
        EngineSoundCheck();
        EngineSound(motorPhysics.CurrentVelocityOffset, motorPhysics.IsGrounded);

        if (motorInput.IsBraking)
        {
            if (motorPhysics.IsGrounded)
            {
                DriftSound(motorPhysics.BikeRB.velocity.magnitude, motorPhysics.MaxSpeed, isDrifting: true);
            }
            else DriftSound(motorPhysics.BikeRB.velocity.magnitude, motorPhysics.MaxSpeed, isDrifting: false);
        }
        else DriftSound(motorPhysics.BikeRB.velocity.magnitude, motorPhysics.MaxSpeed, isDrifting: false);
    }


    private void EngineSoundCheck() // Call whenever player input + velocity > 0 (check only once)
    {

        // Moving (forward / backward)
        if (motorPhysics.CurrentVelocityOffset > 0.01f)
        {
            if (HasInput())
            {
                if (!engineStarted)
                {
                    engineStarted = true;
                    StartEngineSound();
                }

                currentDisengageTime = timeTillEngineDisengage;
            }
        }

        // Stop
        else
        {
            if (HasInput() && engineStarted)
            {
                currentDisengageTime = timeTillEngineDisengage;
            }
            else if (!HasInput() && engineStarted)
            {
                if (currentDisengageTime > 0) currentDisengageTime -= Time.deltaTime;
                else
                {
                    engineStarted = false;
                    DisengageEngineSound();
                }                       
            }
            
        }
    }


    private bool HasInput()
    {
        return motorInput.MoveInput != 0; // Check movement input
    }


    #region ENGINE START AUDIO
    private void StartEngineSound() // Once the player starts moving, this plays.
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
    private void EngineSound(float velocityOffset, bool isGrounded)
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
    private void DriftSound(float velocityMagnitude, float maxSpeed, bool isDrifting)
    {
        if (driftAudioSource.clip == null) return;

        if (!driftAudioSource.isPlaying)
        {
            driftAudioSource.Play();
        }

        float normalizeSpeed = Mathf.Clamp01(velocityMagnitude / maxSpeed);
        float curvedPercentage = SoundManager.Instance.MotorcycleUniversalVolumeStats.DriftVolumeCurve.Evaluate(normalizeSpeed);

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


    private void DisengageEngineSound() // If players stand still for too long, turn engine off.
    {
        FadeEngineSound(FadeOption.FadeOut);
        engineToggleAudioSource.PlayOneShot(keyturnAudio, SoundManager.Instance.MotorcycleUniversalVolumeStats.KeyturnVolume);
    }

    //--------
    #endregion

}

