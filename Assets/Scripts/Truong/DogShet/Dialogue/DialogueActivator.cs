using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [Header("Dialogue Object")]
    [SerializeField] DialogueObject _dialogueObject;
    
    private DialogueObject _currentDialogueObject;
    
    [Header("References")]
    [SerializeField] private GameObject _responseEventHolder;
    [SerializeField] private GameObject _cinemachineObj;

    void Start()
    {
        _currentDialogueObject = _dialogueObject;
    }
    
    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        _dialogueObject = dialogueObject;
    }

    public void UpdateCurrentDialogueObject(DialogueObject dialogueObject)
    {
        _currentDialogueObject = dialogueObject;
        AddDialogueResponseEvents();
    }

    public void Interact(IInteractor interactor)
    {
        if (DialogueUI.Instance.IsOpen) return;

        CursorManager.Instance.ShowCursor();
        CinemachineManager.Instance.SetNewCamera(_cinemachineObj);
        
;        AddDialogueResponseEvents();

        if (interactor != null)
        {
            //.SetCanMove(false);
            //interactor.SetCanInteract(false);
        }
        
        DialogueUI.Instance.ShowDialogue(this, _dialogueObject, () =>
        {
            //interactor.SetCanMove(true);
            //interactor.SetCanInteract(true);
        });
    }

    private void AddDialogueResponseEvents()
    {
        foreach (DialogueResponseEvents dialogueResponseEvents in _responseEventHolder.GetComponents<DialogueResponseEvents>())
        {
            if (dialogueResponseEvents.DialogueObject == _currentDialogueObject)
            {
                DialogueUI.Instance.AddResponseEvents(dialogueResponseEvents.ResponseEvents);
            }
        }
    }
}