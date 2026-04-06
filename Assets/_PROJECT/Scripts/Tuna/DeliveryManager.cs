using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : Singleton<DeliveryManager>
{
    private Order _currentOrder;
    private DeliveryStateMachine _deliveryStateMachine = new();

    private Dictionary<LocationID, LocationNode> _locationNodes = new();

    public Order GetCurrentOrder() => _currentOrder;
    public bool IsOrderAccepted => _deliveryStateMachine.CurrentState == DeliveryState.Accepted;

    public bool IsPickupLocation(LocationID loc)
    {
        return IsOrderAccepted && _currentOrder != null && _currentOrder.PickupLocID == loc;
    }

    public void RegisterStation(LocationID id, LocationNode location)
    {
        _locationNodes[id] = location;
    }

    public void UnregisterStation(LocationID id)
    {
        _locationNodes.Remove(id);
    }

    public List<LocationID> GetAvailablePickupLocations()
    {
        return new List<LocationID>(_locationNodes.Keys);
    }

    public List<LocationID> GetAvailableDropLocations()
    {
        return new List<LocationID>(_locationNodes.Keys);
    }

    public Vector3 GetLocationPosition(LocationID id)
    {
        if (_locationNodes.TryGetValue(id, out var pickup))
            return pickup.transform.position;
        return Vector3.zero;
    }


    public List<LocationID> GetLocationsInArea(AreaID targetArea)
    {
        List<LocationID> validLoc = new List<LocationID>();

        foreach (var loc in _locationNodes)
        {
            if (loc.Value.Area == targetArea)
            {
                validLoc.Add(loc.Key);
            }
        }

        return validLoc;
    }

    public AreaID GetAreaOfLocation(LocationID pickupID)
    {
        if (_locationNodes.TryGetValue(pickupID, out var pickupLoc))
        {
            return pickupLoc.Area;
        }

        return AreaID.HaNoi;
    }


    public void AcceptOrder(Order order)
    {
        if (_deliveryStateMachine.CurrentState == DeliveryState.CarryingPackage)
        {
            return;
        }
        Debug.Log($"Accepted order: {order.OrderID}");
        _currentOrder = order;
        _deliveryStateMachine.AcceptOrder();
    }

    public bool PickupPackage(LocationID id)
    {
        if (_currentOrder == null || _currentOrder.PickupLocID != id)
            return false;

        JobBoardManager.Instance.RemoveJob(_currentOrder);
        _deliveryStateMachine.TryPickupPackage();
        return true;
    }

    public bool DeliverPackage(LocationID id)
    {
        if (_currentOrder == null || _currentOrder.DropLocID != id)
            return false;

        if (_deliveryStateMachine.TryDeliver())
        {
            _currentOrder = null;
            JobBoardManager.Instance.TickTurn();
            return true;
        }

        return false;
    }
}