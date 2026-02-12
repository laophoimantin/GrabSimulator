using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum DeliveryState { PendingOrder, AcceptedOrder, PackagePickedUp, Delivered }

public class DeliveryStates : MonoBehaviour
{
    private OrderInfoSO order;

    public DeliveryStates(OrderInfoSO order)
    {
        this.order = order;
    }

    // Centralized state change
    private void ChangeState(DeliveryState newState)
    {
        order.CurrentDeliveryState = newState;
        OnStateEnter(newState);
    }

    // Called whenever we enter a new state
    private void OnStateEnter(DeliveryState state)
    {
        switch (state)
        {
            case DeliveryState.PendingOrder:
                Debug.Log("Entered PendingOrder: waiting for player to accept.");
                break;
            case DeliveryState.AcceptedOrder:
                Debug.Log("Entered AcceptedOrder: show pickup locations.");
                break;
            case DeliveryState.PackagePickedUp:
                Debug.Log("Entered PackagePickedUp: show delivery points.");
                break;
            case DeliveryState.Delivered:
                Debug.Log("Entered Delivered: reward player and reset cycle.");
                break;
        }
    }

    // Public methods to trigger transitions
    public void AcceptOrder()
    {
        if (order.CurrentDeliveryState == DeliveryState.PendingOrder)
            ChangeState(DeliveryState.AcceptedOrder);
    }

    public void PickupPackage()
    {
        if (order.CurrentDeliveryState == DeliveryState.AcceptedOrder)
            ChangeState(DeliveryState.PackagePickedUp);
    }

    public void DeliverPackage()
    {
        if (order.CurrentDeliveryState == DeliveryState.PackagePickedUp)
            ChangeState(DeliveryState.Delivered);
    }

    public void ResetCycle()
    {
        if (order.CurrentDeliveryState == DeliveryState.Delivered)
            ChangeState(DeliveryState.PendingOrder);
    }
}
