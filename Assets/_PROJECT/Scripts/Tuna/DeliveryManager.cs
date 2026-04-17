using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : Singleton<DeliveryManager>
{
    private Order _currentOrder;
    private DeliveryStateMachine _deliveryStateMachine = new();
    public DeliveryState GetCurrentState() => _deliveryStateMachine.CurrentState;
    private Dictionary<LocationID, LocationNode> _locationNodes = new();

    public Order GetCurrentOrder() => _currentOrder;
    public bool IsOrderAccepted => _deliveryStateMachine.CurrentState == DeliveryState.Accepted;

    public static Action OnDeliveryUpdated;

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

    public Transform GetLocationTransform(LocationID id)
    {
        if (_locationNodes.TryGetValue(id, out var node))
            return node.transform;
        return null;
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

        if (QuestArrow.Instance != null)
        {
            Transform pickupPoint = GetLocationTransform(order.PickupLocID);
            QuestArrow.Instance.SetObjective(pickupPoint);
        }

        OnDeliveryUpdated?.Invoke();
        //if (_deliveryStateMachine.CurrentState == DeliveryState.CarryingPackage)
        //{
        //	return;
        //}

        //Debug.Log($"Accepted order: {order.OrderID}");
        //_currentOrder = order;
        //_deliveryStateMachine.AcceptOrder();
        //OnDeliveryUpdated?.Invoke();
    }

    public bool PickupPackage(LocationID id)
    {
        if (_currentOrder == null || _currentOrder.PickupLocID != id)
            return false;

        if (_deliveryStateMachine.TryPickupPackage())
        {
            JobBoardManager.Instance.RemoveJob(_currentOrder);

            if (QuestArrow.Instance != null)
            {
                Transform dropPoint = GetLocationTransform(_currentOrder.DropLocID);
                QuestArrow.Instance.SetObjective(dropPoint);
            }

            OnDeliveryUpdated?.Invoke();
            return true;
        }

        return _currentOrder.PickupLocID == id;
        //if (_currentOrder == null || _currentOrder.PickupLocID != id)
        //    return false;

        //if (_deliveryStateMachine.TryPickupPackage())
        //{
        //    JobBoardManager.Instance.RemoveJob(_currentOrder);
        //    OnDeliveryUpdated?.Invoke();
        //    return true;
        //}

        //return _currentOrder.PickupLocID == id;
    }

    public bool DeliverPackage(LocationID id)
    {
        if (_currentOrder == null || _currentOrder.DropLocID != id)
            return false;

        if (_deliveryStateMachine.TryDeliver())
        {
            WalletSystem.Instance.AddCoins(_currentOrder.Reward);
            _currentOrder = null;

            if (QuestArrow.Instance != null)
            {
                QuestArrow.Instance.SetObjective(null);
            }

            JobBoardManager.Instance.TickTurn();
            OnDeliveryUpdated?.Invoke();
            return true;
        }

        return false;


        //if (_currentOrder == null || _currentOrder.DropLocID != id)
        //    return false;

        //if (_deliveryStateMachine.TryDeliver())
        //{
        //    WalletSystem.Instance.AddCoins(_currentOrder.Reward);
        //    _currentOrder = null;

        //    JobBoardManager.Instance.TickTurn();
        //    OnDeliveryUpdated?.Invoke();
        //    return true;
        //}

        //return false;
    }
}