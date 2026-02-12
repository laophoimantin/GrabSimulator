using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    public OrderInfoSO currentOrder;
    private DeliveryStates deliveryStates;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public void StartDelivery(OrderInfoSO order)
    {
        if (currentOrder != null)
        {
            Debug.LogWarning("Already have an active delivery. Finish it before starting a new one.");
            return;
        }
        currentOrder = order;
        deliveryStates = new DeliveryStates(order); 
        deliveryStates.AcceptOrder(); // Automatically transition to AcceptedOrder when starting
        Debug.Log($"Started delivery for order: {order.OrderID}");
    }



}