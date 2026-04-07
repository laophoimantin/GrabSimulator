using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DialogueActivator : MonoBehaviour
{
    public void SpeakRandom(DialogueDataSO[] dialogues, Action onFinishedCallback = null)
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            onFinishedCallback?.Invoke();
            return;
        }

        DialogueDataSO diag = dialogues[Random.Range(0, dialogues.Length)];
        DialogueController.Instance.StartDialogue(diag, onFinishedCallback);
    }
}
