using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueGameEventListenerPro : MonoBehaviour
{
    [SerializeField] private List<EventBinding> _bindings;

    private void OnEnable()
    {
        foreach (var binding in _bindings)
        {
            binding.Subscribe();
        }
    }

    private void OnDisable()
    {
        foreach (var binding in _bindings)
        {
            binding.Unsubscribe();
        }
    }

    // This tiny class handles the logic for a single event in the list
    [Serializable]
    public class EventBinding : IGameEventListener
    {
        [Tooltip("The Event to listen for")]
        public DialogueGameEvent Event;
        
        [Tooltip("What happens when this specific event fires")]
        public UnityEvent Response;

        public void Subscribe()
        {
            if (Event != null) Event.RegisterListener(this);
        }

        public void Unsubscribe()
        {
            if (Event != null) Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            Response.Invoke();
        }
    }
}