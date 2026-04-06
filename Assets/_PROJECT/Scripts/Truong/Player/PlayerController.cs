using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerInteractor _interactor;
    [SerializeField] private PlayerVisualController _visualController;
    [SerializeField] private Transform _handPos;

    public Transform HandPos => _handPos;
    public void LockInteraction()
    {
        _interactor.LockInteraction();
    }
    
    public void UnlockInteraction()
    {
        _interactor.UnlockInteraction();
    }

    public void LockMovement()
    {
        _movement.SetMovementLock(true);
    }

    public void UnlockMovement()
    {
        _movement.SetMovementLock(false);
    }
    
    public void HideModel()
    {
        _visualController.SetModelState(false);
    }

    public void ShowModel()
    {
        _visualController.SetModelState(true);
    }

}