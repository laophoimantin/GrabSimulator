using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global 
{
    public class Motorcycle
    {
        #region COLLISION VARIABLE
        public static float MinimumCollisionForce { get; } = 4f;
        #endregion

    }

    public class Dialogue
    {
        #region EMOTION PITCH VALUE - [TESTING]
        public static float Normal { get; } = 1.85f;
        public static float Happy { get; } = 2f;
        public static float Moody { get; } = 1.5f;
        #endregion
    }

}


#region Motorbike Sound Controller ENUM
//-------------------------------------

public enum FadeOption
{
    FadeIn,
    FadeOut,
}

//--------
#endregion

//[SPACE]

#region Sound Manager STRUCT
//--------------------------

[Serializable]
public struct MotorbikeGameplaySoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}


[Serializable]
public struct EnvironmentSoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}


[Serializable]
public struct DialogueSoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}


[Serializable]
public struct MotorbikeSoundSettingList
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

    public float TimeTillEngineDisengage { get => _timeTillEngineDisengage; }
    [SerializeField] private float _timeTillEngineDisengage;
}


//--------
#endregion
