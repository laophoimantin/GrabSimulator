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


        #region LANDING VARIABLE
        public static float MinimumLandingForce { get; } = 2f;
        public static float MaximumLandingSpeed { get; } = 50f; // Change this later
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


#region [ENUM] Motorbike Type
//---------------------------

public enum MotorbikeType
{
    Cub,
}

//--------
#endregion



#region [ENUM] Motorbike Sound Controller 
//---------------------------------------

public enum FadeOption
{
    FadeIn,
    FadeOut,
}

//--------
#endregion



#region [STRUCT] Sound Manager
//----------------------------

[Serializable]
public struct MotorbikeGameplaySoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}


[Serializable]
public struct DeliverySoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}


[Serializable]
public struct AmbienceSoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}


[Serializable]
public struct OccasionalSoundList
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


//--------
#endregion
