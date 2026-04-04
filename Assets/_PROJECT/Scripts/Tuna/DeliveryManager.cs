using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : Singleton<DeliveryManager>
{
    private OrderInfoSO _currentOrder;
    private DeliveryStates _deliveryStates = new();

    private Dictionary<LocationID, PickupLoc> _pickupMap = new();
    private Dictionary<LocationID, DropLoc> _dropMap = new();

    public void RegisterPickup(LocationID id, PickupLoc loc)
    {
        _pickupMap[id] = loc;
    }

    public void UnregisterPickup(LocationID id)
    {
        _pickupMap.Remove(id);
    }

    public void RegisterDrop(LocationID id, DropLoc loc)
    {
        _dropMap[id] = loc;
    }

    public void UnregisterDrop(LocationID id)
    {
        _dropMap.Remove(id);
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

    void OnDestroy()
    {
        PickupLoc.OnPickupEntered -= PickupPackage;
        DropLoc.OnDropEntered -= DeliverPackage;
    }

    public void AcceptOrder(OrderInfoSO order)
    {
        Debug.Log($"Accepting order {order.OrderName} at {order.PickupLocationID} to {order.DropLocationID}");
        if (_currentOrder != null)
        {
            Debug.LogWarning("Already have an active delivery. Finish it before starting a new one.");
            return;
        }

        if (order == null)
        {
            Debug.LogWarning("No order to accept");
            return;
        }

        if (!_pickupMap.TryGetValue(order.PickupLocationID, out var loc))
        {
            Debug.LogWarning("Pickup location not registered");
            return;
        }

        if (!_dropMap.ContainsKey(order.DropLocationID))
        {
            Debug.LogWarning("Drop location not registered");
            return;
        }

        Debug.Log($"Accepted order: {order.OrderName}");
        _currentOrder = order;
        loc.SpawnPackage(order.OrderObject);
        _deliveryStates.AcceptOrder();
    }

    private void PickupPackage(LocationID id)
    {
        if (_currentOrder == null || _currentOrder.PickupLocationID != id) 
            return;

        if (_deliveryStates.TryPickupPackage())
        {
            if (_pickupMap.TryGetValue(id, out var loc))
            {
                loc.RemovePackage(); 
            }

            
            
            
            
            // Example
            if (_dropMap.TryGetValue(_currentOrder.DropLocationID, out var dropLoc))
            {
                // dropLoc.EnableHighlightMarker(true);
            }
        }
    }

    private void DeliverPackage(LocationID id)
    {
        if (_currentOrder == null || _currentOrder.DropLocationID != id) return;

        if (_deliveryStates.TryDeliver())
        {
            Debug.Log("Package delivered");
            if (_dropMap.TryGetValue(_currentOrder.DropLocationID, out var dropLoc))
            {
                // dropLoc.EnableHighlightMarker(false); // Example
            }

            _currentOrder = null;
        }
    }
}