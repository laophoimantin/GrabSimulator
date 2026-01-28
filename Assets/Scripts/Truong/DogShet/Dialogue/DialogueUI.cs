using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class DialogueUI : Singleton<DialogueUI>
{
    [SerializeField] GameObject _dialogueBox;
    [SerializeField] private TMP_Text _textLabel;

    public bool IsOpen { get; private set; }

    [SerializeField] private ResponseHandler _responseHandler;
    [SerializeField] private TypewriterEffect _typewriterEffect;

    [SerializeField] private KeyCode _keyToContinue = KeyCode.Mouse0;

    private event Action onDialogueClosed;
    
    private DialogueActivator _currentDialogueActivator;
    

    void Start()
    {
        if (_typewriterEffect == null)
            _typewriterEffect = GetComponent<TypewriterEffect>();
        if (_responseHandler == null)
            _responseHandler = GetComponent<ResponseHandler>();

        _dialogueBox.SetActive(false);
        _textLabel.text = string.Empty;
        IsOpen = false;
    }


    public void ShowDialogue(DialogueActivator activator, DialogueObject dialogueObject, Action onFinishedEvent = null)
    {
        _currentDialogueActivator = activator;
        onDialogueClosed = onFinishedEvent;

        IsOpen = true;
        OpenDialogueBox();
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        _responseHandler.AddResponseEvents(responseEvents);
    }


    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            yield return RunTypingEffect(dialogue);

            _textLabel.text = dialogue;

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses)
                break;

            yield return null;
            yield return new WaitUntil(() => Input.GetKeyDown(_keyToContinue));
        }

        if (dialogueObject.HasResponses)
        {
            _responseHandler.ShowResponse(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }
    

    public void StepToNode(DialogueObject dialogueObject)
    {
        StopAllCoroutines();
        _currentDialogueActivator.UpdateCurrentDialogueObject(dialogueObject);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator RunTypingEffect(string dialogue)
    {
        _typewriterEffect.Run(dialogue, _textLabel);

        while (_typewriterEffect.IsRunning)
        {
            yield return null;

            if (Input.GetKeyDown(_keyToContinue))
            {
                _typewriterEffect.Stop();
            }
        }
    }

    private void OpenDialogueBox()
    {
        _dialogueBox.SetActive(true);
    }

    public void CloseDialogueBox()
    {
        
        onDialogueClosed?.Invoke();
        onDialogueClosed = null;

        IsOpen = false;
        _dialogueBox.SetActive(false);
        _textLabel.text = string.Empty;
        CursorManager.Instance.HideCursor();
        
    }
}