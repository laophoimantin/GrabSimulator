using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] GameObject _dialogueBox;
    [SerializeField] private TMP_Text _textLabel;
    [SerializeField] private DialogueObject _dialogueObject;

    public bool IsOpen { get; private set; }

    [SerializeField] private ResponseHandler _responseHandler;
    [SerializeField] private TypewriterEffect _typewriterEffect;

    [SerializeField] KeyCode _keyToContinue = KeyCode.Space;

    void Start()
    {
        if (_typewriterEffect == null)
            _typewriterEffect = GetComponent<TypewriterEffect>();
        if (_responseHandler == null)
            _responseHandler = GetComponent<ResponseHandler>();
        CloseDialogueBox();
        //ShowDialogue(_dialogueObject);
    }


    public void ShowDialogue(DialogueObject dialogueObject)
    {
        IsOpen = true;
        OpenDialogueBox();
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        // foreach (string dialogue in dialogueObject.Dialogue)
        // {
        //     yield return _typewriterEffect.Run(dialogue, _textLabel);
        //     yield return new WaitUntil(() => Input.GetKeyDown(_keyToContinue));
        // }



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

    private void CloseDialogueBox()
    {
        IsOpen = false;
        _dialogueBox.SetActive(false);
        _textLabel.text = string.Empty;
    }
}
