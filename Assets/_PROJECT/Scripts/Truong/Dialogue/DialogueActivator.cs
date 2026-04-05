using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DialogueActivator : MonoBehaviour
{
    [SerializeField] private DialogueDataSO[] _idleDialogues;
    [SerializeField] private DialogueDataSO[] _pickupDialogues;

    public void PlayIdleDialogue()
    {
        DialogueDataSO diag = _idleDialogues[Random.Range(0, _idleDialogues.Length)];
        PlayDialogue(diag);
    }
    
    public void PlayPickupDialogue(Action onFinishedCallback)
    {
        DialogueDataSO diag = _pickupDialogues[Random.Range(0, _pickupDialogues.Length)];
        PlayDialogue(diag, onFinishedCallback);
    }
    
    
    
    
    private void PlayDialogue(DialogueDataSO data, Action onFinishedCallback = null)
    {
        if (data == null)
            return;
        
        DialogueController.Instance.StartDialogue(data, onFinishedCallback);
    }
}
