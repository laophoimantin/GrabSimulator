using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MotorbikeSoundController : MonoBehaviour
{

    [Header("Motorbike Sound SO Reference")]
    [SerializeField] private MotorbikeSoundSO motorbikeSoundSO;


    [Header("References")]
    [SerializeField] private MotorbikeEntrySystem motorbikeEntrySystem;
    [SerializeField] private MotorbikePhysics motorPhysics;
    [SerializeField] private MotorbikeInput motorInput;


    [Header("Engine Audio Sources")]
    [SerializeField] private AudioSource engineToggleAudioSource;
    [SerializeField] private AudioSource engineRunAudioSource;


    [Header("Gameplay Audio Sources")]
    [SerializeField] private AudioSource driftAudioSource;
    [SerializeField] private AudioSource collisionAudioSource;
    [SerializeField] private AudioSource landingAudioSource;
    [SerializeField] private AudioSource honkingAudioSource;


    [Header("Sound of what motorcycle?")]
    [SerializeField] private MotorbikeType motorcycleSoundType;




    // Key Turn Sound References
    private AudioClip keyturnAudio;


    // Drifting Sound References
    private AudioClip driftAudio;
    private AudioClip collisionAudio;
    private AudioClip landingAudio;
    private AudioClip honkingAudio;


    // Drift Volume
    private float minDriftVolume;
    private float maxDriftVolume;


    // Collision Volume
    private float minCollisionVolume;
    private float maxCollisionVolume;


    // Collision Pitch
    private float minCollisionPitch;
    private float maxCollisionPitch;


    // Landing Volume
    private float minLandingVolume;
    private float maxLandingVolume;


    // Landing Pitch
    private float minLandingPitch;
    private float maxLandingPitch;


    // Honking Volume
    private float minHonkingVolume;
    private float maxHonkingVolume;


    // Sound Variable
    private float currentDisengageTime;


    // Honking Sound
    private readonly float honkFadeSpeed = 10f;


    // Variable
    private bool engineStarted = false;


    // Engine Coroutines
    private Coroutine startEngineCoroutine = null; // for engine start
    private Coroutine fadeEngineCoroutine = null; // for fading in/out engine sound



    private void Start()
    {
        Initialization();
    }
    private void Initialization()
    {
        if (motorbikeSoundSO == null) 
        { 
            Debug.Log("Assign Motorbike SO");
            return;
        }


        if (engineToggleAudioSource != null) engineToggleAudioSource.playOnAwake = false;
        if (engineRunAudioSource != null) engineRunAudioSource.playOnAwake = false;
        if (collisionAudioSource != null) collisionAudioSource.playOnAwake = false;
        if (driftAudioSource != null) driftAudioSource.playOnAwake = false;
        if (landingAudioSource != null) landingAudioSource.playOnAwake = false;
        if (honkingAudioSource != null) honkingAudioSource.playOnAwake = false;


        minDriftVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MinDriftVolume;
        maxDriftVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MaxDriftVolume;

        minCollisionVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MinCollisionVolume;
        maxCollisionVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MaxCollisionVolume;

        minCollisionPitch = SoundManager.Instance.MotorcycleUniversalVolumeStats.MinCollisionPitch;
        maxCollisionPitch = SoundManager.Instance.MotorcycleUniversalVolumeStats.MaxCollisionPitch;

        minLandingVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MinLandingVolume;
        maxLandingVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MaxLandingVolume;

        minLandingPitch = SoundManager.Instance.MotorcycleUniversalVolumeStats.MinLandingPitch;
        maxLandingPitch = SoundManager.Instance.MotorcycleUniversalVolumeStats.MaxLandingPitch;

        minHonkingVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MinHonkingVolume;
        maxHonkingVolume = SoundManager.Instance.MotorcycleUniversalVolumeStats.MaxHonkingVolume;


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


        landingAudio = SoundManager.Instance.GetMotorbikeGameplaySound(MotorbikeGameplaySoundType.Landing_Sound);
        if (landingAudio == null) Debug.Log("Assign Landing_Sound in SoundManager");
        else
        {
            landingAudioSource.clip = landingAudio;
            landingAudioSource.loop = false;
            landingAudioSource.volume = 0f;
        }


        honkingAudio = SoundManager.Instance.GetMotorbikeGameplaySound(MotorbikeGameplaySoundType.Honking_Sound);
        if (honkingAudio == null) Debug.Log("Assign Honking_Sound in SoundManager");
        else
        {
            honkingAudioSource.clip = honkingAudio;
            honkingAudioSource.loop = true;
            honkingAudioSource.volume = 0f;
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

        HonkingSound();
    }


    private void EngineSoundCheck() // Call whenever player input + velocity > 0 (check only once)
    {

        // Moving (forward / backward)
        if (motorPhysics.CurrentVelocityOffset > 0.01f)
        {
            if (HasInput() && !engineStarted)
            {
                StartEngineSound();
                currentDisengageTime = motorPhysics.MotorbikeStatsSO.TimeTillEngineDisengage;
            }
        }

        // Stop
        else
        {
            if (HasInput() && engineStarted)
            {
                currentDisengageTime = motorPhysics.MotorbikeStatsSO.TimeTillEngineDisengage;
            }
            else if (!HasInput() && engineStarted)
            {
                if (currentDisengageTime > 0) currentDisengageTime -= Time.deltaTime;
                else
                {
                    if (motorbikeEntrySystem?.State == VehicleState.Empty)
                    {
                        DisengageEngineSound();
                    }

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
        engineStarted = true;

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

        EngineAudio(motorbikeSoundSO.EngineStartAudio, isStartEngine: true);
        yield return new WaitForSeconds(motorPhysics.MotorbikeStatsSO.TimeTillEngineStart);

        EngineAudio(motorbikeSoundSO.EngineRunAudio, isStartEngine: false);
        FadeEngineSound(FadeOption.FadeIn);

        startEngineCoroutine = null;
    }
    #endregion



    #region ENGINE RUN SOUND
    private void EngineSound(float velocityOffset, bool isGrounded)
    {
        if (engineRunAudioSource.clip == null) return;

        float clampedVelosityOffset = Mathf.Max(0f, velocityOffset);
        float speedMain = Mathf.Lerp(motorbikeSoundSO.MinPitch, motorbikeSoundSO.MaxPitch, clampedVelosityOffset);

        float volumeMain = Mathf.Lerp(motorbikeSoundSO.MinEngineRunVolume, motorbikeSoundSO.MaxEngineRunVolume, clampedVelosityOffset);

        engineRunAudioSource.pitch = speedMain;

        float fadeSpeed = 3f;
        float targetVolume = isGrounded ? volumeMain : volumeMain / 2f;

        float tempVolume = Mathf.MoveTowards(engineRunAudioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
        engineRunAudioSource.volume = tempVolume;
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

        if (driftAudioSource.isPlaying && driftAudioSource.volume <= 0.01f)
        {
            driftAudioSource.Pause();
            driftAudioSource.volume = 0f;
        }
    }
    #endregion



    #region COLLISION SOUND
    public void CollisionSound(float impactSpeed, float maxSpeed)
    {
        Debug.Log("Goddamn");

        if (collisionAudioSource == null) return;

        float crashSeverityPercentage = Mathf.Clamp01(impactSpeed / maxSpeed);
        float targetVolume = Mathf.Lerp(minCollisionVolume, maxCollisionVolume, crashSeverityPercentage);
        float targetPitch = Mathf.Lerp(maxCollisionPitch, minCollisionPitch, crashSeverityPercentage);

        collisionAudioSource.pitch = targetPitch;
        collisionAudioSource.volume = targetVolume;
        collisionAudioSource.PlayOneShot(collisionAudio);
    }
    #endregion



    #region LANDING SOUND
    public void LandingSound(float impactSpeed, float maxSpeed)
    {
        Debug.Log("Goddamn");

        if (collisionAudioSource == null) return;

        float crashSeverityPercentage = Mathf.Clamp01(impactSpeed / maxSpeed);
        float targetVolume = Mathf.Lerp(minLandingVolume, maxLandingVolume, crashSeverityPercentage);
        float targetPitch = Mathf.Lerp(maxLandingPitch, minLandingPitch, crashSeverityPercentage);

        landingAudioSource.pitch = targetPitch;
        landingAudioSource.volume = targetVolume;
        landingAudioSource.PlayOneShot(landingAudio);
    }

    #endregion



    #region HONKING SOUND - Sunshine Contribution
    private void HonkingSound()
    {
        if (motorInput.IsHonking && !honkingAudioSource.isPlaying)
        {
            honkingAudioSource.Play();
        }

        float targetVolume = motorInput.IsHonking ? 0.4f : 0f;

        honkingAudioSource.volume = Mathf.MoveTowards(honkingAudioSource.volume, targetVolume, honkFadeSpeed * Time.deltaTime);

        if (!motorInput.IsHonking && honkingAudioSource.isPlaying && honkingAudioSource.volume == 0f)
        {
            honkingAudioSource.Pause();
        }
    }
    #endregion



    #region Helpers & Extra Functionalities
    //-------------------------------------

    private void EngineAudio(AudioClip engineAudio, bool isStartEngine)
    {
        if (engineAudio == null) return;

        if (isStartEngine)
        {
            engineToggleAudioSource.PlayOneShot(engineAudio, motorbikeSoundSO.MaxEngineStartVolume);
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

        float targetVolume = option == FadeOption.FadeIn ? motorbikeSoundSO.MaxEngineRunVolume: 0f;

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
        engineStarted = false;

        FadeEngineSound(FadeOption.FadeOut);
        engineToggleAudioSource.PlayOneShot(keyturnAudio, SoundManager.Instance.MotorcycleUniversalVolumeStats.KeyturnVolume);
    }

    //--------
    #endregion

}

