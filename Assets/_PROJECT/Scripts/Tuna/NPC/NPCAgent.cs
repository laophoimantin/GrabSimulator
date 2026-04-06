using UnityEngine;

public class NPCAgent : MonoBehaviour, IInteractable
{
    [Header("Dialogues Config")]
    [SerializeField] private LocationNode _myLocation;
    [SerializeField] private DialogueActivator _myMouth;

    public void Interact(IInteractor interactor)
    {
        if (DeliveryManager.Instance.IsPickupLocation(_myLocation.ID))
        {
            _myMouth.PlayPickupDialogue(OnPickupDialogueFinished);
        }
        else
        {
            _myMouth.PlayIdleDialogue();
        }
    }

    private void OnPickupDialogueFinished()
    {
        _myLocation.SpawnPackage();
    }
}
