using UnityEngine;

namespace DefaultNamespace
{
    public class DialogueActivator : MonoBehaviour, IInteractable
    {
        [SerializeField] DialogueObject _dialogueObject;

        public void UpdateDialogueObject(DialogueObject dialogueObject)
        {
            _dialogueObject = dialogueObject;
        }
        
        public void Interact(PlayerInteractor player)
        {
            foreach (DialogueResponseEvents dialogueResponseEvents in GetComponents<DialogueResponseEvents>())
            {
                if (dialogueResponseEvents.DialogueObject == _dialogueObject)
                {
                    player.DialogueUI.AddResponseEvents(dialogueResponseEvents.ResponseEvents);
                }
            }

            player.DialogueUI.ShowDialogue(_dialogueObject);
        }

        public string GetInteractionPrompt()
        {
            return "Dialogue Activated";
        }
    }
}