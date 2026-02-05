using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerInteractor _interactor;

    
    
    public void SetPlayerMoveState(bool isLocked)
    {
        _movement.LockMovement(isLocked);
    }
}