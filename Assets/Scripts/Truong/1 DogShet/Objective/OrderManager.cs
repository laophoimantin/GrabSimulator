using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : Singleton<OrderManager>
{
    public enum OrderState
    {
        NoOrder,
        Accepted,
        PackagePickedUp,
        Delivered
    }

    [Header("References")]
    //[SerializeField] private MotorbikeController _motorbikeController;
    [SerializeField] private CarManager _carController;

    private Dictionary<LocationID, Transform> _pickupMap = new();
    private Dictionary<LocationID, Transform> _dropoffMap = new();

    // The current active job
    public DeliveryOrder CurrentOrder { get; private set; }
    public OrderState CurrentState { get; private set; } = OrderState.NoOrder;


    // Prefire
    public event Action<DeliveryOrder> OnOrderAccepted;
    public event Action OnPackagePickedUp;
    public event Action OnOrderCompleted;


    public void AcceptOrder(DeliveryOrder order)
    {
        if (CurrentState != OrderState.NoOrder)
        {
            Debug.Log("You already have a job.");
            return;
        }

        CurrentOrder = order;
        CurrentState = OrderState.Accepted;
        Debug.Log($"Accepted Order: {order.Description}. Go to {order.PickupLocationID}");
        
        Transform target = GetPickupTransform(order.PickupLocationID);
        QuestArrow.Instance.SetObjective(target);

        OnOrderAccepted?.Invoke(order);
    }

    public void PickupPackage(GameObject package)
    {
        if (CurrentState != OrderState.Accepted) return;

        CurrentState = OrderState.PackagePickedUp;
        Debug.Log("Got the goods!");
        _carController.GetPackage(package);
        
        Transform target = GetDropoffTransform(CurrentOrder.DropoffLocationID);
        QuestArrow.Instance.SetObjective(target);

        OnPackagePickedUp?.Invoke();
    }


    public void DeliverPackage(Transform dropPoint)
    {
        if (CurrentState != OrderState.PackagePickedUp) return;

        CurrentState = OrderState.Delivered;
        Debug.Log($"Delivered! You earned ${CurrentOrder.RewardMoney}");
        _carController.DropPackage(dropPoint);
        QuestArrow.Instance.SetObjective(null);
        
        OnOrderCompleted?.Invoke();
        // Reset for next job
        CurrentOrder = null;
        CurrentState = OrderState.NoOrder;
    }

    
    public void RegisterPickup(LocationID id, Transform t)
    {
        if (!_pickupMap.ContainsKey(id)) _pickupMap.Add(id, t);
    }

    public void RegisterDropoff(LocationID id, Transform t)
    {
        if (!_dropoffMap.ContainsKey(id)) _dropoffMap.Add(id, t);
    }
    

    public Transform GetPickupTransform(LocationID id)
    {
        return _pickupMap.ContainsKey(id) ? _pickupMap[id] : null;
    }

    public Transform GetDropoffTransform(LocationID id)
    {
        return _dropoffMap.ContainsKey(id) ? _dropoffMap[id] : null;
    }
}