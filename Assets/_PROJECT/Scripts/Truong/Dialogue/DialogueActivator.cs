using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueDataSO _dialogueData;
    public void Interact(IInteractor interactor)
    {
        Debug.Log("Interacting with DialogueActivator");
        DialogueController.Instance.StartDialogue(_dialogueData);
    }
}
