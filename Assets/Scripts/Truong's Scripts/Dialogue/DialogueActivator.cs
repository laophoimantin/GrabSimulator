using UnityEngine;

namespace DefaultNamespace
{
    public class DialogueActivator : MonoBehaviour, IInteractable
    {
        [SerializeField] DialogueObject _dialogueObject;
        public void Interact(PlayerInteractor player)
        {
            player.DialogueUI.ShowDialogue(_dialogueObject);
        }

        public string GetInteractionPrompt()
        {
            return "Dialogue Activated";
        }
    }
}