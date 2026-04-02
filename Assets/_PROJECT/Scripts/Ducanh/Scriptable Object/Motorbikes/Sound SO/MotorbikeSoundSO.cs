using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Motorbike Sound Data", menuName = "Motorbike Sound SO/Motorbike Sound")]
public class MotorbikeSoundSO : ScriptableObject
{
    public MotorbikeType MotorbikeType;

    public AudioClip EngineStartAudio;
    public AudioClip EngineRunAudio;

    [Range(-10, 10)] public float MinPitch;
    [Range(-10, 10)] public float MaxPitch;

    public float MaxEngineStartVolume;
    public float MaxEngineRunVolume;
}
