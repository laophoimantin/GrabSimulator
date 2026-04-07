using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;

public enum MotorbikeGameplaySoundType
{
    // FOR MOTORBIKE
    Drift_Sound,
    Collision_Sound,
    Landing_Sound,
    Key_Turn_Sound,
    Honking_Sound
}

public enum DeliverySoundType
{
    // FOR ITEM INTERACTION
    Item_Pick_Up,
    Item_Drop_Off
}

public enum AmbienceSoundType
{
    // FOR ENVIRONMENT
    Wind
}

public enum OccasionalSoundType
{
    // FOR ENVIRONMENT
    None,
    Bird_Chirping
}

public enum DialogueSoundType
{
    // FOR DIALOGUE
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z
}

public enum UISoundType
{
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
    public AudioMixer MasterMixer
    {
        get => masterMixer;
    } // Use this for settings in the future.


    [Header("[ SOUNDS ]")]
    [Space]
    [Header("Motorbike Gameplay")]
    [SerializeField] private MotorbikeGameplaySoundList[] motorbikeGameplaySoundList;

    [Space]
    [Header("Delivery Gameplay")]
    [SerializeField] private DeliverySoundList[] deliverySoundList;

    [Space]
    [Header("Ambience")]
    [SerializeField] private AmbienceSoundList[] ambienceSoundList;

    [Space]
    [Header("Occassional")]
    [SerializeField] private OccasionalSoundList[] occasionalSoundList;

    [Space]
    [Header("Dialogue")]
    [SerializeField] private DialogueSoundList[] dialogueSoundList;
    public DialogueSoundList[] DialogueSoundList
    {
        get => dialogueSoundList;
    }

    [Space]
    [Header("3D Settings")]
    [SerializeField] private float maxSoundDistance = 20f;
    [SerializeField] private float spatialBlend = 1f;
    [Space]
    [Space]
    [Header("[ MOTORCYCLE UNIVERSAL VOLUME SETTINGS ]")]
    [SerializeField] MotorcycleUniversalVolumeSettings motorcycleUniversalVolumeStats;
    public MotorcycleUniversalVolumeSettings MotorcycleUniversalVolumeStats
    {
        get => motorcycleUniversalVolumeStats;
    }

    [Serializable]
    public class MotorcycleUniversalVolumeSettings
    {
        [Header("KEY SOUND\n" +
                "---------")]
        [Header("Key Turn Volume")]
        [SerializeField] private float _keyturnVolume;
        public float KeyturnVolume
        {
            get => _keyturnVolume;
        }
        [Space]
        [Space]
        [Space]
        [Header("DRIFT SOUND\n" +
                "-----------")]
        [Header("Drift Volume")]
        [SerializeField] private float _minDriftVolume;
        [SerializeField] private float _maxDriftVolume;
        public float MinDriftVolume
        {
            get => _minDriftVolume;
        }
        public float MaxDriftVolume
        {
            get => _maxDriftVolume;
        }


        [Header("Drift Volume Interpolation")]
        [SerializeField] private AnimationCurve _driftVolumeCurve;
        public AnimationCurve DriftVolumeCurve
        {
            get => _driftVolumeCurve;
        }
        [Space]
        [Space]
        [Space]
        [Header("COLLISION SOUND\n" +
                "---------------")]
        [Header("Collision Volume")]
        [SerializeField] private float _minCollisionVolume;
        [SerializeField] private float _maxCollisionVolume;
        public float MinCollisionVolume
        {
            get => _minCollisionVolume;
        }
        public float MaxCollisionVolume
        {
            get => _maxCollisionVolume;
        }


        [Header("Collision Pitch Values")]
        [SerializeField] private float _minCollisionPitch; // Original Pitch
        [SerializeField] private float _maxCollisionPitch; // For cases of light collision
        public float MinCollisionPitch
        {
            get => _minCollisionPitch;
        }
        public float MaxCollisionPitch
        {
            get => _maxCollisionPitch;
        }
        [Space]
        [Space]
        [Space]
        [Header("LANDING SOUND\n" +
                "-------------")]
        [Header("Landing Volume")]
        [SerializeField] private float _minLandingVolume;
        [SerializeField] private float _maxLandingVolume;
        public float MinLandingVolume => _minLandingVolume;
        public float MaxLandingVolume => _maxLandingVolume;


        [Header("Landing Pitch Values")]
        [SerializeField] private float _minLandingPitch;
        [SerializeField] private float _maxLandingPitch;
        public float MinLandingPitch => _minLandingPitch;
        public float MaxLandingPitch => _maxLandingPitch;
        [Space]
        [Space]
        [Space]
        [Header("HONKING SOUND\n" +
                "-------------")]
        [Header("Honking Volume")]
        [SerializeField] private float _minHonkingVolume;
        [SerializeField] private float _maxHonkingVolume;
        public float MinHonkingVolume => _minHonkingVolume;
        public float MaxHonkingVolume => _maxHonkingVolume;
    }


    // For UI / OneShots
    private AudioSource main2DSource;


    // CHANNEL 1: SCENE LOOPS (Sirens, Machines) - Dies on Load
    private Dictionary<Enum, AudioSource> activeSceneLoops = new Dictionary<Enum, AudioSource>();


    // CHANNEL 2: GLOBAL TRACKS (Music/Ambience) - Persists automatically
    private Dictionary<Enum, AudioSource> currentAmbienceSources = new Dictionary<Enum, AudioSource>();


    // CHANNEL 3: OCCASIONAL SOUNDS (Random Loops)
    private Dictionary<(Transform, Enum), Coroutine> activeOccasionalRoutines = new Dictionary<(Transform, Enum), Coroutine>();


    // CHANNEL 4: 3D ONE-SHOT TRACKER
    private List<GameObject> active3DOneShots = new List<GameObject>();


    // MASTER DICTIONARY
    private Dictionary<string, AudioClip[]> masterSoundDictionary = new Dictionary<string, AudioClip[]>();


    private class ActiveZoneData
    {
        public AmbienceZone ZoneScript;
        public Enum Sound;
        public float Volume;
        public int Priority;
    }

    private List<ActiveZoneData> overlappingZones = new List<ActiveZoneData>();
    private AmbienceZone[] allZonesInScene;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialization();
    }


    // MOST IMPORTANT -> Decides which enum sound list can be played/accessed through SoundManager!
    private void Initialization()
    {
        foreach (var motorbikeGameplaySound in motorbikeGameplaySoundList)
        {
            masterSoundDictionary.Add(motorbikeGameplaySound.name, motorbikeGameplaySound.Sounds);
        }

        foreach (var deliverySound in deliverySoundList)
        {
            masterSoundDictionary.Add(deliverySound.name, deliverySound.Sounds);
        }

        foreach (var dialogueSound in dialogueSoundList)
        {
            masterSoundDictionary.Add(dialogueSound.name, dialogueSound.Sounds);
        }

        foreach (var environmentSound in ambienceSoundList)
        {
            masterSoundDictionary.Add(environmentSound.name, environmentSound.Sounds);
        }

        foreach (var occasionalSound in occasionalSoundList)
        {
            masterSoundDictionary.Add(occasionalSound.name, occasionalSound.Sounds);
        }
    }


    void Start()
    {
        main2DSource = GetComponent<AudioSource>();
        main2DSource.spatialBlend = 0f;

        allZonesInScene = FindObjectsOfType<AmbienceZone>();
    }


    void OnValidate()
    {
        // Initializing GAMEPLAY-related sounds
        string[] motorbikeGameplaySoundNames = Enum.GetNames(typeof(MotorbikeGameplaySoundType));
        Array.Resize(ref motorbikeGameplaySoundList, motorbikeGameplaySoundNames.Length);
        for (int i = 0; i < motorbikeGameplaySoundList.Length; i++) motorbikeGameplaySoundList[i].name = motorbikeGameplaySoundNames[i];


        // Initializing DELIVERY-related sounds
        string[] deliverySoundNames = Enum.GetNames(typeof(DeliverySoundType));
        Array.Resize(ref deliverySoundList, deliverySoundNames.Length);
        for (int i = 0; i < deliverySoundList.Length; i++) deliverySoundList[i].name = deliverySoundNames[i];


        // Initializing AMBIENCE-related sounds
        string[] ambienceSoundNames = Enum.GetNames(typeof(AmbienceSoundType));
        Array.Resize(ref ambienceSoundList, ambienceSoundNames.Length);
        for (int i = 0; i < ambienceSoundList.Length; i++) ambienceSoundList[i].name = ambienceSoundNames[i];


        // Initializing OCCASSIONAL-related sounds
        string[] occassionalSoundNames = Enum.GetNames(typeof(OccasionalSoundType));
        Array.Resize(ref occasionalSoundList, occassionalSoundNames.Length);
        for (int i = 0; i < occasionalSoundList.Length; i++) occasionalSoundList[i].name = occassionalSoundNames[i];


        // Initializing DIALOGUE-related sounds
        string[] dialogueSoundNames = Enum.GetNames(typeof(DialogueSoundType));
        Array.Resize(ref dialogueSoundList, dialogueSoundNames.Length);
        for (int i = 0; i < dialogueSoundList.Length; i++) dialogueSoundList[i].name = dialogueSoundNames[i];
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

        Button[] buttons = FindObjectsOfType<Button>(true);
        Debug.Log($"Found {buttons.Length} buttons. Injecting sound chips...");

        //foreach (Button btn in buttons)
        //{
        //    if (btn.gameObject.GetComponent<ButtonSoundInject>() == null)
        //    {
        //        btn.gameObject.AddComponent<ButtonSoundInject>();
        //    }
        //}
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

    public void PlaySound2D<T>(T sound, float volume = 1) where T : Enum
    {
        AudioClip clip = GetRandomClip(sound);
        if (clip != null) main2DSource.PlayOneShot(clip, volume);
    }

    public void PlaySound3D<T>(T sound, Vector3 position, float volume = 1) where T : Enum
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
    public void PlaySceneLoop<T>(T sound, Transform parent, float volume = 1) where T : Enum
    {
        if (activeSceneLoops.ContainsKey(sound)) return; // Already playing this loop

        AudioSource source = CreateAudioObject(sound, parent, volume, true, parent != null);
        activeSceneLoops.Add(sound, source);
    }

    public void StopSceneLoop<T>(T sound) where T : Enum
    {
        if (activeSceneLoops.TryGetValue(sound, out AudioSource source))
        {
            if (source != null) Destroy(source.gameObject);
            activeSceneLoops.Remove(sound);
        }
    }

    #endregion


    #region Channel 2: Global Ambience & Music (Persistent)

    public void PlayAmbience<T>(T sound, float volume = 1) where T : Enum
    {
        if (currentAmbienceSources.ContainsKey(sound))
        {
            return;
        }

        AudioSource audio = CreateAudioObject(sound, transform, volume, true, false);
        currentAmbienceSources.Add(sound, audio);
    }

    public void PlayCustomAudioSourceAmbience<T>(T sound, AudioSource audioSource, float volume = 1) where T : Enum
    {
        if (currentAmbienceSources.ContainsKey(sound))
        {
            return;
        }

        AudioSource audio = CreateAudioObject(sound, transform, volume, true, false);
        currentAmbienceSources.Add(sound, audio);
    }

    public void StopAmbience<T>(T sound) where T : Enum
    {
        if (!currentAmbienceSources.TryGetValue(sound, out AudioSource audioSource))
        {
            return;
        }

        if (audioSource != null)
        {
            Destroy(audioSource.gameObject);
        }

        currentAmbienceSources.Remove(sound);
    }

    #endregion


    #region Volumetric Zone Ambience Logic (The Conductor)

    public void RegisterZone(AmbienceZone zone, int priority)
    {
        if (!overlappingZones.Exists(z => z.ZoneScript == zone))
        {
            overlappingZones.Add(new ActiveZoneData { ZoneScript = zone, Priority = priority });
        }

        EvaluateOverlappingZones();
    }

    public void UnregisterZone(AmbienceZone zone)
    {
        overlappingZones.RemoveAll(z => z.ZoneScript == zone);
        EvaluateOverlappingZones();
    }

    private void EvaluateOverlappingZones()
    {
        if (overlappingZones.Count == 0)
        {
            if (allZonesInScene != null)
            {
                foreach (var z in allZonesInScene)
                {
                    if (z != null) z.SetZoneState(false);
                }
            }

            return;
        }

        overlappingZones.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        int highestPriority = overlappingZones[0].Priority;

        foreach (var z in overlappingZones)
        {
            bool isBoss = (z.Priority == highestPriority);
            z.ZoneScript.SetZoneState(isBoss);
        }
    }

    public AudioClip GetAmbienceClip(AmbienceSoundType sound)
    {
        return GetRandomClip(sound); // Repurposing your existing private method!
    }

    #endregion


    #region Channel 3: Occasional Sounds

    public void PlayOccasionalSound2D<T>(T sound, int percentage, float checkInterval = 5f, float volume = 1f, bool allowOverwrite = true) where T : Enum
    {
        // For 2D sounds, the "Parent" is just the SoundManager itself.
        var dictKey = ((Transform)null, sound);

        if (!allowOverwrite && activeOccasionalRoutines.ContainsKey(dictKey)) return;

        StopOccasionalSound(null, sound);

        Coroutine routine = StartCoroutine(OccasionalRoutine2D(sound, percentage, checkInterval, volume));
        activeOccasionalRoutines.Add(dictKey, routine);
    }

    private IEnumerator OccasionalRoutine2D<T>(T sound, int percentage, float interval, float volume) where T : Enum
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

    public void PlayOccasionalSound3D<T>(T sound, Transform parent, int percentage, float checkInterval = 5f, float volume = 1f, bool allowOverwrite = true) where T : Enum
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

    private IEnumerator OccasionalRoutine3D<T>(T sound, Transform parent, (Transform, Enum) dictKey, int percentage, float interval, float volume) where T : Enum
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
    public void StopOccasionalSound<T>(Transform parent, T sound) where T : Enum
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

    private AudioSource CreateAudioObject<T>(T sound, Transform parent, float volume, bool loop, bool is3D) where T : Enum
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


    private AudioClip GetRandomClip<T>(T sound) where T : Enum
    {
        string targetSoundName = sound.ToString();

        if (masterSoundDictionary.TryGetValue(targetSoundName, out AudioClip[] clips))
        {
            if (clips == null || clips.Length == 0)
            {
                Debug.LogWarning($"SoundManager: No audio clips assigned for [{targetSoundName}]!");
                return null;
            }

            return clips[UnityEngine.Random.Range(0, clips.Length)];
        }

        Debug.LogError($"SoundManager: Missing sound config for [{targetSoundName}]. Check your Inspector typos!");
        return null;
    }

    #endregion


    #region Getters

    //-------------


    #region For Dialogue Sound Controller

    public Dictionary<char, AudioClip> GetDialogueSoundDictionary()
    {
        Dictionary<char, AudioClip> tempDictionary = new Dictionary<char, AudioClip>();

        foreach (DialogueSoundType syllableEnum in Enum.GetValues(typeof(DialogueSoundType)))
        {
            AudioClip randomSyllableClip = GetRandomClip(syllableEnum);

            if (randomSyllableClip != null)
            {
                char rawLetter = syllableEnum.ToString()[0];
                char lowerChar = char.ToLower(rawLetter);

                tempDictionary.Add(lowerChar, randomSyllableClip);
            }
            else
            {
                Debug.LogWarning($"SoundManager: Missing audio for {syllableEnum}!");
            }
        }

        return tempDictionary;
    }

    #endregion


    #region For Motorbike Sound Controller

    public AudioClip GetMotorbikeGameplaySound(MotorbikeGameplaySoundType soundType)
    {
        return GetRandomClip(soundType);
    }

    #endregion


    //--------

    #endregion
}


//#region UI BUTTON SOUND INJECTION
////-------------------------------

//public class ButtonSoundInject : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
//{
//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        Button btn = GetComponent<Button>();
//        if (btn != null && btn.interactable)
//        {
//            if (btn.gameObject.layer == LayerMask.NameToLayer("UI"))
//            {
//                SoundManager.Instance.PlaySound2D(UISoundType.Button_Hover, 1f);  // Used to be 0.5
//            }
//        }
//    }

//    public void OnPointerClick(PointerEventData eventData)
//    {
//        Button btn = GetComponent<Button>();
//        if (btn != null && btn.interactable)
//        {
//            if (btn.gameObject.layer == LayerMask.NameToLayer("UI"))
//            {
//                SoundManager.Instance.PlaySound2D(UISoundType.Button_Selection, 0.7f); // Used  to be 0.2
//            }
//        }
//    }
//}

////--------
//#endregion