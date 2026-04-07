using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DialogueSoundController : MonoBehaviour
{
    // References
    private Dictionary<char, AudioClip> syllableAudioClipDictionary = new Dictionary<char, AudioClip>();
    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
        else Debug.Log("Please assign AudioSource");

        syllableAudioClipDictionary = SoundManager.Instance.GetDialogueSoundDictionary();
    }

    public void DialogueSoundGenerator(char letter, float pitch, float volume)
    {
        volume = Mathf.Clamp01(volume);
        char convertedLetter = char.ToLower(letter);

        if (syllableAudioClipDictionary.TryGetValue(convertedLetter, out AudioClip syllableSound))
        {
            audioSource.volume = volume;
            audioSource.pitch = pitch + Random.Range(-0.25f, 0.25f);
            audioSource.clip = syllableSound;

            audioSource.Play();
        }
        else
        {
            Debug.Log($"Dictionary doesn't contain \"{convertedLetter}\", please double check within SoundManager");
        }
    }
}