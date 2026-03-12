using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : Singleton<DeliveryManager>
{
    private OrderInfoSO _currentOrder;
    private DeliveryStates _deliveryStates = new();
    private Dictionary<LocationID, PickupLoc> _pickupMap = new();
    
    public void Register(LocationID id, PickupLoc loc)
    {
        _pickupMap[id] = loc;
    }

    public void Unregister(LocationID id)
    {
        _pickupMap.Remove(id);
    }

    void OnEnable()
    {
        PickupLoc.OnPickupEntered += PickupPackage;
        DropLoc.OnDropEntered += DeliverPackage;
    }
    void OnDisable()
    {
        PickupLoc.OnPickupEntered -= PickupPackage;
        DropLoc.OnDropEntered -= DeliverPackage;
    }
    
    public void AcceptOrder(OrderInfoSO order)
    {
        if (_currentOrder != null)
        {
            Debug.LogWarning("Already have an active delivery. Finish it before starting a new one.");
            return;
        }
        Debug.Log("Accepted order");
        _currentOrder = order;
        
        if (_pickupMap.TryGetValue(order.PickupLoc, out var loc))
        {
            loc.SpawnPackage(order.OrderObject);
        }
        
        _deliveryStates.AcceptOrder();
    }
    
    private void PickupPackage(LocationID id)
    {
        if (_currentOrder == null)
            return;

        if (_currentOrder.PickupLoc != id)
            return;

        if (_deliveryStates.TryPickupPackage())
        {
            Debug.Log("Package picked up");
        }
    }

    private void DeliverPackage(LocationID id)
    {
        if (_currentOrder == null)
            return;
        
        if (_currentOrder.DropLoc != id)
            return;

        if (_deliveryStates.TryDeliver())
        {
            _currentOrder = null;
        }
    }
}

