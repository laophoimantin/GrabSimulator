using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : Singleton<DeliveryManager>
{
    public OrderInfoSO CurrentOrder;
    public DeliveryStates DeliveryStates;

    public bool HasOrder => CurrentOrder != null;
    public void StartDelivery(OrderInfoSO order)
    {
        if (CurrentOrder != null)
        {
            Debug.LogWarning("Already have an active delivery. Finish it before starting a new one.");
            return;
        }
        CurrentOrder = order;
        DeliveryStates.AssignNewOrder(CurrentOrder);
        DeliveryStates.AcceptOrder(); // Automatically transition to AcceptedOrder when starting
        Debug.Log($"Started delivery for order: {order.OrderName}");
    }

    public void PickupPackage()
    {
        if (CurrentOrder == null)
        {
            Debug.LogWarning("No active delivery to pick up.");
            return;
        }

        //add any additional logic for package special delivery rules here

        DeliveryStates.PickupPackage();
    }

    public void DeliverPackage()
    {
        if (CurrentOrder == null)
        {
            Debug.LogWarning("No active delivery to deliver.");
            return;
        }

        //add any additional logic for delivery special rules here

        DeliveryStates.DeliverPackage();
    }

    public void CompleteDelivery()
    {
        if (CurrentOrder == null)
        {
            Debug.LogWarning("No active delivery to complete.");
            return;
        }
        else if (DeliveryStates.CurrentDeliveryState != DeliveryState.Delivered)
        {
            Debug.LogWarning("Package not delivered yet!");
            return;
        }
        else if (DeliveryStates.CurrentDeliveryState == DeliveryState.Delivered)
        {
            Debug.Log($"Completed delivery for order: {CurrentOrder.OrderName}");
            DeliveryStates.ResetCycle(); // Reset the state machine for the next order
            CurrentOrder = null;
        }
    }
}

