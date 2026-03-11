using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float _typewriterSpeed = 50f;
    
    public bool IsRunning { get; private set; }

    private readonly List<Punctuation> _punctuations = new()
    {
        new Punctuation( new HashSet<char>() { '.', '!', '?' }, 0.6f ),
        new Punctuation( new HashSet<char>() { ',', ';', ':' }, 0.3f)
    };
    
    private Coroutine _typingCoroutine;

    public void Run(string textToType, TMP_Text textLabel)
    {
        _typingCoroutine = StartCoroutine(TypeText(textToType, textLabel));
    }
    
    public void Stop()
    {
        StopCoroutine(_typingCoroutine);
        IsRunning = false;
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        IsRunning = true;
            
        textLabel.text = string.Empty;
        
        float time = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;
            
            time += Time.deltaTime * _typewriterSpeed; 
            
            charIndex = Mathf.FloorToInt(time);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = i >= textToType.Length - 1;

                textLabel.text = textToType.Substring(0, i + 1);
                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i + 1], out _))
                {
                    yield return new WaitForSeconds(waitTime);
                }
            }

            textLabel.text = textToType.Substring(0, charIndex);
            yield return null;
        }

        IsRunning = false;
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (Punctuation punctuationCategory in _punctuations)
        {
            if (punctuationCategory.Punctuations.Contains(character))
            {
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }

        waitTime = 0;
        return false; 
    }
    
    private readonly struct Punctuation
    {
        public readonly HashSet<char> Punctuations;
        public readonly float WaitTime;
        public Punctuation(HashSet<char> punctuations, float waitTime)
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
}
