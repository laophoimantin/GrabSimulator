using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [Header("Dialogue Object")]
    [SerializeField] DialogueObject _dialogueObject;
    
    private DialogueObject _currentDialogueObject;
    
    [Header("References")]
    [SerializeField] private GameObject _responseEventHolder;

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

    public void Interact(PlayerInteractor playerInteractor)
    {
        if (DialogueUI.Instance.IsOpen) return;
        
        AddDialogueResponseEvents();

        playerInteractor.PlayerMovement.SetCanMove(false);
        playerInteractor.SetCanInteract(false);
        DialogueUI.Instance.ShowDialogue(this, _dialogueObject, () =>
        {
            playerInteractor.PlayerMovement.SetCanMove(true);
            playerInteractor.SetCanInteract(true);
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

    public string GetInteractionPrompt()
    {
        return "Dialogue Activated";
    }
}