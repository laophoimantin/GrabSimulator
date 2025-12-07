using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ResponseEvent
{
    [HideInInspector] public string Name;
    [SerializeField] private UnityEvent _onPickedResponse;
    public UnityEvent OnPickedResponse => _onPickedResponse;
}