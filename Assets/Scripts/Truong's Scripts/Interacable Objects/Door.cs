using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private bool _isOpen;
    public void Interact( PlayerInteractor player)
    {
        if (_isOpen) CloseChest();
        else OpenChest();
    }

    public string GetInteractionPrompt()
    {
        return _isOpen ? "Close Door" : "Open Door";
    }

    private void OpenChest()
    {
        _isOpen = true;
        Debug.Log("CRackkk........!");
        transform.Rotate(-90, 0, 0);
    }

    private void CloseChest()
    {
        _isOpen = false;
        Debug.Log("Slam!");
        transform.Rotate(90, 0, 0);
    }
}
