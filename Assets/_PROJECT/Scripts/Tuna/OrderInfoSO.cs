using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new order", menuName = "delivery/order")]

public class OrderInfoSO : ScriptableObject
{
    // Private Fields
    [Header("Order Details")]
    [SerializeField] private string _orderName;
    [SerializeField] private string _clientName;

    [SerializeField] private LocationID _pickupLoc;
    [SerializeField] private LocationID _dropLoc;

    [Header("Special Instructions")]
    [SerializeField] private string _description;

    [Header("Payment")]
    [SerializeField] private int _cashPayment;
    
    [Header("Goods")]
    [SerializeField] private GameObject _orderObject;

    // Public Fields
    public string OrderName => _orderName;
    public string ClientName => _clientName;
    public LocationID PickupLoc => _pickupLoc;
    public LocationID DropLoc => _dropLoc;
    public string Description => _description;
    public int CashPayment => _cashPayment;
    public GameObject OrderObject => _orderObject;
}

