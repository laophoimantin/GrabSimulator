using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;


public enum MotorcycleSoundType
{
    // FOR IDENTIFYING MOTORCYCLES
    Cub,
}


public enum SoundType
{
    // FOR GAMEPLAY
    Drift_Sound, 
    Collision_Sound,
    Key_Turn_Sound,

    // FOR UI
    Button_Hover, 
    Button_Selection,

}


[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }


    [Header("Mixer")]
    [SerializeField] private AudioMixer masterMixer;
    public AudioMixer MasterMixer { get => masterMixer; } // Use this for settings in the future.


    [Header("[ SOUNDS ]")]
    [Space]
    [SerializeField] private SoundList[] soundList;
    [SerializeField] private MotorcycleSound[] motorcycleSoundList;
    public MotorcycleSound[] MotorcycleSoundList { get => motorcycleSoundList; }


    [Header("3D Settings")]
    [SerializeField] private float maxSoundDistance = 20f;
    [SerializeField] private float spatialBlend = 1f;
    [Space]
    [Space]

    [Header("[ MOTORCYCLE GENERAL VOLUME SETTINGS ]")]
    [SerializeField] Motorcycle motorcycleVolumeStats;
    public Motorcycle MotorcycleVolumeStats { get => motorcycleVolumeStats; }

    [Serializable]
    public class Motorcycle
    {
        [Header("Key Turn Volume")]
        [SerializeField] private float _keyturnVolume;
        public float KeyturnVolume { get => _keyturnVolume; }
        [Space]


        [Header("Drift Volume")]
        [SerializeField] private float _minDriftVolume;
        [SerializeField] private float _maxDriftVolume;
        public float MinDriftVolume { get => _minDriftVolume; }
        public float MaxDriftVolume { get => _maxDriftVolume; }
        [Space]


        [Header("Drift Volume Interpolation")]
        [SerializeField] private AnimationCurve _driftVolumeCurve;
        public AnimationCurve DriftVolumeCurve { get => _driftVolumeCurve; }
        [Space]


        [Header("Collision Volume")]
        [SerializeField] private float _minCollisionVolume;
        [SerializeField] private float _maxCollisionVolume;
        public float MinCollisionVolume { get => _minCollisionVolume; }
        public float MaxCollisionVolume { get => _maxCollisionVolume; }
        [Space]


        [Header("Collision Pitch Values")]
        [SerializeField] private float _minCollisionPitch; // Original Pitch
        [SerializeField] private float _maxCollisionPitch; // For cases of light collision
        public float MinCollisionPitch { get => _minCollisionPitch; }
        public float MaxCollisionPitch { get => _maxCollisionPitch; }

    }

    // For UI / OneShots
    private AudioSource main2DSource; 


    // CHANNEL 1: SCENE LOOPS (Sirens, Machines) - Dies on Load
    private Dictionary<SoundType, AudioSource> activeSceneLoops = new Dictionary<SoundType, AudioSource>();


    // CHANNEL 2: GLOBAL TRACKS (Music/Ambience) - Persists automatically
    private Dictionary<SoundType, AudioSource> currentAmbienceSources = new Dictionary<SoundType, AudioSource>();
    private AudioSource currentAmbienceSource;
    private SoundType currentAmbienceType;


    // CHANNEL 3: OCCASIONAL SOUNDS (Random Loops)
    private Dictionary<(Transform, SoundType), Coroutine> activeOccasionalRoutines = new Dictionary<(Transform, SoundType), Coroutine>();


    // CHANNEL 4: 3D ONE-SHOT TRACKER
    private List<GameObject> active3DOneShots = new List<GameObject>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        main2DSource = GetComponent<AudioSource>();
        main2DSource.spatialBlend = 0f;
    }

    void OnValidate()
    {
        // Initializing normal sounds
        string[] soundNames = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, soundNames.Length);
        for (int i = 0; i < soundList.Length; i++) soundList[i].name = soundNames[i];


        // Initializing motorcycle-related sounds
        string[] motorcycleSoundNames = Enum.GetNames(typeof(MotorcycleSoundType));
        Array.Resize(ref motorcycleSoundList, motorcycleSoundNames.Length);
        for (int i = 0; i < motorcycleSoundList.Length; i++) motorcycleSoundList[i].name = motorcycleSoundNames[i];
    }



    #region Scene Management
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;

    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllSceneLoops();
        StopAllOccasionalSounds();

        if (main2DSource != null)
            main2DSource.Stop();

        foreach (GameObject go in active3DOneShots)
        {
            if (go != null) Destroy(go);
        }
        active3DOneShots.Clear();

        Button[] buttons = GameObject.FindObjectsOfType<Button>(true);
        Debug.Log($"Found {buttons.Length} buttons. Injecting sound chips...");

        foreach (Button btn in buttons)
        {
            if (btn.gameObject.GetComponent<ButtonSoundInject>() == null)
            {
                btn.gameObject.AddComponent<ButtonSoundInject>();
            }
        }
    }

    private void StopAllSceneLoops()
    {
        foreach (var kvp in activeSceneLoops)
        {
            if (kvp.Value != null) Destroy(kvp.Value.gameObject);
        }
        activeSceneLoops.Clear();
    }
    #endregion


    #region Channel 1: One Shots & Scene Loops
    public void PlaySound2D(SoundType sound, float volume = 1)
    {
        AudioClip clip = GetRandomClip(sound);
        if (clip != null) main2DSource.PlayOneShot(clip, volume);
    }

    public void PlaySound3D(SoundType sound, Vector3 position, float volume = 1)
    {
        AudioClip clip = GetRandomClip(sound);
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio_" + sound);
        tempGO.transform.position = position;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;

        source.spatialBlend = spatialBlend;
        source.maxDistance = maxSoundDistance;
        source.rolloffMode = AudioRolloffMode.Linear;

        source.Play();

        // 1. ADD IT TO THE HIT-LIST
        active3DOneShots.Add(tempGO);

        Destroy(tempGO, clip.length + 0.1f);
    }

    // These die when you leave the scene
    public void PlaySceneLoop(SoundType sound, Transform parent, float volume = 1)
    {
        if (activeSceneLoops.ContainsKey(sound)) return; // Already playing this loop

        AudioSource source = CreateAudioObject(sound, parent, volume, true, parent != null);
        activeSceneLoops.Add(sound, source);
    }

    public void StopSceneLoop(SoundType sound)
    {
        if (activeSceneLoops.TryGetValue(sound, out AudioSource source))
        {
            if (source != null) Destroy(source.gameObject);
            activeSceneLoops.Remove(sound);
        }
    }

    #endregion


    #region Channel 2: Global Ambience & Music (Persistent)
    public void PlayAmbience(SoundType sound, float volume = 1)
    {
        if (currentAmbienceSources.ContainsKey(sound))
        {
            // Optional: You could fade the volume here if you wanted to update it!
            return;
        }

        AudioSource audio = CreateAudioObject(sound, transform, volume, true, false);
        currentAmbienceSources.Add(sound, audio);
    }

    public void StopAmbience(SoundType sound)
    {
        if (!currentAmbienceSources.TryGetValue(sound, out AudioSource audioSource))
        {
            return; // Nothing to stop
        }

        if (audioSource != null)
        {
            Destroy(audioSource.gameObject);
        }

        currentAmbienceSources.Remove(sound);
    }

    #endregion


    #region Channel 3: Occasional Sounds

    public void PlayOccasionalSound2D(SoundType sound, int percentage, float checkInterval = 5f, float volume = 1f, bool allowOverwrite = true)
    {
        // For 2D sounds, the "Parent" is just the SoundManager itself.
        var dictKey = ((Transform)null, sound);

        if (!allowOverwrite && activeOccasionalRoutines.ContainsKey(dictKey)) return;

        StopOccasionalSound(null, sound);

        Coroutine routine = StartCoroutine(OccasionalRoutine2D(sound, percentage, checkInterval, volume));
        activeOccasionalRoutines.Add(dictKey, routine);
    }

    private IEnumerator OccasionalRoutine2D(SoundType sound, int percentage, float interval, float volume)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            if (UnityEngine.Random.Range(0, 101) <= percentage)
            {
                AudioSource source = CreateAudioObject(sound, null, volume, false, false);
                if (source != null)
                {
                    yield return new WaitForSeconds(source.clip.length);
                    if (source != null) Destroy(source.gameObject);
                }
            }
        }
    }

    public void PlayOccasionalSound3D(SoundType sound, Transform parent, int percentage, float checkInterval = 5f, float volume = 1f, bool allowOverwrite = true)
    {
        if (parent == null)
        {
            Debug.LogWarning($"Trying to play 3D Occasional Sound {sound} but parent is null!");
            return;
        }

        // We bind the Sound to the specific Transform!
        var dictKey = (parent, sound);

        if (!allowOverwrite && activeOccasionalRoutines.ContainsKey(dictKey)) return;

        StopOccasionalSound(parent, sound);

        Coroutine routine = StartCoroutine(OccasionalRoutine3D(sound, parent, dictKey, percentage, checkInterval, volume));
        activeOccasionalRoutines.Add(dictKey, routine);
    }

    private IEnumerator OccasionalRoutine3D(SoundType sound, Transform parent, (Transform, SoundType) dictKey, int percentage, float interval, float volume)
    {
        while (true)
        {
            if (parent == null)
            {
                // The NPC died or was destroyed. Clean up our dictionary!
                activeOccasionalRoutines.Remove(dictKey);
                yield break;
            }

            yield return new WaitForSeconds(interval);

            if (parent == null) break;

            if (UnityEngine.Random.Range(0, 101) <= percentage)
            {
                AudioSource source = CreateAudioObject(sound, parent, volume, false, true);
                if (source != null)
                {
                    yield return new WaitForSeconds(source.clip.length);
                    if (source != null) Destroy(source.gameObject);
                }
            }
        }
    }

    // NOTICE: We must ask for the Transform here now, so we know WHOSE cough to stop!
    public void StopOccasionalSound(Transform parent, SoundType sound)
    {
        var dictKey = (parent, sound);

        if (activeOccasionalRoutines.TryGetValue(dictKey, out Coroutine routine))
        {
            if (routine != null) StopCoroutine(routine);
            activeOccasionalRoutines.Remove(dictKey);
        }
    }

    public void StopAllOccasionalSounds()
    {
        foreach (var kvp in activeOccasionalRoutines)
        {
            if (kvp.Value != null) StopCoroutine(kvp.Value);
        }
        activeOccasionalRoutines.Clear();
    }
    #endregion


    #region Helpers
    private AudioSource CreateAudioObject(SoundType sound, Transform parent, float volume, bool loop, bool is3D)
    {
        AudioClip clip = GetRandomClip(sound);
        if (clip == null) return null;

        GameObject obj = new GameObject("Audio_" + sound);

        if (parent != null)
        {
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
        }

        AudioSource src = obj.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;
        src.loop = loop;

        if (is3D)
        {
            src.spatialBlend = spatialBlend;
            src.maxDistance = maxSoundDistance;
            src.rolloffMode = AudioRolloffMode.Linear;
        }
        else src.spatialBlend = 0f;

        src.Play();
        return src;
    }

    public AudioClip GetRandomClip(SoundType sound)
    {
        int index = (int)sound;

        // 1. Safety Check: Is the index valid?
        if (index < 0 || index >= soundList.Length)
        {
            Debug.LogError($"SoundManager: Missing sound config for [{sound}]. Update the SoundManager inspector!");
            return null;
        }

        // 2. Safety Check: Is the clip array empty?
        AudioClip[] clips = soundList[index].Sounds;
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"SoundManager: No audio clips assigned for [{sound}]!");
            return null;
        }

        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }

    #endregion

}


#region SOUND STRUCTS
//-------------------

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}


[Serializable]  
public struct MotorcycleSound
{
    [HideInInspector] public string name;
    public AudioClip EngineRun { get => _engineRun; }
    public AudioClip EngineStart { get => _engineStart; }

    [SerializeField] private AudioClip _engineStart;
    [SerializeField] private AudioClip _engineRun;

    public float MinPitch { get => _minPitch; }
    public float MaxPitch { get => _maxPitch; }

    [Range(-10, 10)]
    [SerializeField] private float _minPitch;
    [Range(-10, 10)]
    [SerializeField] private float _maxPitch;

    public float MaxEngineStartVolume { get => _maxEngineStartVolume; }
    [SerializeField] private float _maxEngineStartVolume;

    public float MaxEngineVolume { get => _maxEngineVolume; }
    [SerializeField] private float _maxEngineVolume;

    public float TimeTillEngineRun { get => _timeTillEngineRun; }

    [SerializeField] private float _timeTillEngineRun;
}

//--------
#endregion


#region UI BUTTON SOUND INJECTION
//-------------------------------

public class ButtonSoundInject : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Button btn = GetComponent<Button>();
        if (btn != null && btn.interactable)
        {
            if (btn.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                SoundManager.Instance.PlaySound2D(SoundType.Button_Hover, 1f);  // Used to be 0.5
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Button btn = GetComponent<Button>();
        if (btn != null && btn.interactable)
        {
            if (btn.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                SoundManager.Instance.PlaySound2D(SoundType.Button_Selection, 0.7f); // Used  to be 0.2
            }
        }
    }
}

//--------
#endregion
