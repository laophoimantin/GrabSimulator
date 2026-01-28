using System;
using UnityEngine;

public class DialogueResponseEvents : MonoBehaviour
{
    [SerializeField] private DialogueObject _dialogueObject;
    [SerializeField] private ResponseEvent[] _responseEvents;

    public DialogueObject DialogueObject => _dialogueObject;
    public ResponseEvent[] ResponseEvents => _responseEvents;

    public void OnValidate()
    {
        if (_dialogueObject == null) return;
        if (_dialogueObject.Responses == null) return;
        if (_responseEvents != null && _responseEvents.Length == _dialogueObject.Responses.Length) return;

        if (_responseEvents == null)
        {
            _responseEvents = new ResponseEvent[_dialogueObject.Responses.Length];
        }
        else
        {
            Array.Resize(ref _responseEvents, _dialogueObject.Responses.Length);
        }

        for (int i = 0; i < _dialogueObject.Responses.Length; i++)
        {
            Response response = _dialogueObject.Responses[i];

            if (_responseEvents[i] != null)
            {
                _responseEvents[i].Name = response.ResponseText;
                continue;
            }

            _responseEvents[i] = new ResponseEvent() { Name = response.ResponseText };
        }
    }
}