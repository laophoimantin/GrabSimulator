using System;
using UnityEngine;

public class OrderManager : Singleton<OrderManager>
{
    public enum OrderState { NoOrder, Accepted, PackagePickedUp, Delivered }
    
    [Header("References")]
    [SerializeField] private MotorbikeController _motorbikeController;    

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
        
        OnOrderAccepted?.Invoke(order);
    }

    public void PickupPackage(GameObject package)
    {
        if (CurrentState != OrderState.Accepted) return;

        CurrentState = OrderState.PackagePickedUp;
        Debug.Log("Got the goods!");
        _motorbikeController.GetPackage(package);
        
        OnPackagePickedUp?.Invoke();
    }

    
    public void DeliverPackage(Transform dropPoint)
    {
        if (CurrentState != OrderState.PackagePickedUp) return;

        CurrentState = OrderState.Delivered;
        Debug.Log($"Delivered! You earned ${CurrentOrder.RewardMoney}");
        _motorbikeController.DropPackage(dropPoint);
        
        OnOrderCompleted?.Invoke();
        
        // Reset for next job
        CurrentOrder = null;
        CurrentState = OrderState.NoOrder;
    }
}
