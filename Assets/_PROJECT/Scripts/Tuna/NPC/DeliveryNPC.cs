using UnityEngine;

public class DeliveryNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private LocationNode _myLocation;
    
    [Header("Dialogues")]
    [SerializeField] private DialogueDataSO[] _idleDialogues;
    [SerializeField] private DialogueDataSO[] _pickupDialogues;
    
    [SerializeField] private DialogueActivator _myMouth;

    public void Interact(IInteractor interactor)
    {
        if (DeliveryManager.Instance.IsPickupLocation(_myLocation.ID))
        {
            _myMouth.SpeakRandom(_pickupDialogues, _myLocation.SpawnPackage);
        }
        else
        {
            _myMouth.SpeakRandom(_idleDialogues);
        }
    }

    private void OnPickupDialogueFinished()
    {
        _myLocation.SpawnPackage();
    }
}
