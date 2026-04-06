using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<<< HEAD:Assets/_PROJECT/Scripts/Tuna/StateChanges/DeliveryStates.cs
public class DeliveryStates
========

public enum DeliveryState
{
    Pending,
    Accepted,
    CarryingPackage
}

public class DeliveryStateMachine
>>>>>>>> main:Assets/_PROJECT/Scripts/Tuna/DeliveryStateMachine.cs
{
    private DeliveryState _currentDeliveryState;
    public DeliveryState CurrentState => _currentDeliveryState;

    public DeliveryStateMachine()
    {
        _currentDeliveryState = DeliveryState.Pending;
    }

    // Centralized state change
    private void ChangeState(DeliveryState newState)
    {
        _currentDeliveryState = newState;
        OnStateEnter(newState);
    }

    // Called whenever we enter a new state
    private void OnStateEnter(DeliveryState state)
    {
        switch (state)
        {
            case DeliveryState.Pending:
                Debug.Log("Entered PendingOrder: waiting for player to accept.");
                break;
            case DeliveryState.Accepted:
                Debug.Log("Entered AcceptedOrder: show pickup locations.");
                break;
            case DeliveryState.CarryingPackage:
                Debug.Log("Entered PackagePickedUp: show delivery points.");
                break;
        }
    }

    public void AcceptOrder()
    {
        if (_currentDeliveryState == DeliveryState.Pending || _currentDeliveryState == DeliveryState.Accepted)
            ChangeState(DeliveryState.Accepted);
    }

    public bool TryPickupPackage()
    {
        if (_currentDeliveryState != DeliveryState.Accepted)
            return false;
        ChangeState(DeliveryState.CarryingPackage);
        return true;
    }

    public bool TryDeliver()
    {
        if (_currentDeliveryState != DeliveryState.CarryingPackage)
            return false;

        ChangeState(DeliveryState.Pending);
        return true;
    }
}