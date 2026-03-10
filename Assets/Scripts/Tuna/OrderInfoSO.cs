using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new order", menuName = "delivery/order")]

public class OrderInfoSO : ScriptableObject
{
    // Private Fields
    [Header("Order Details")]
    private string _orderName;
    private string _clientName;

    //For more complexity we can add multiple delivery locations per order
    private LocationID _pickupLocation;
    private LocationID _dropLocation;

    private string[] _items;

    [Header("Special Instructions")]

    //Maybe tool reqirements for some specific orders?

    private string _description;

    [Header("Payment")]
    private int _cashPayment;





    public string OrderName => _orderName;
    public string ClientName => _clientName;

    public LocationID PickupLocation => _pickupLocation;
    public LocationID DropLocation => _dropLocation;

    public string[] Items => _items;

    public string Description => _description;
    public int CashPayment => _cashPayment;

    //Maybe some special rewards like special item or tools here?
}

