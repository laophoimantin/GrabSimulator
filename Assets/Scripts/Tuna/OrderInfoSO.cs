using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderInfoSO : ScriptableObject
{
    [field: SerializeField] public string OrderID { get; private set; }

    [Header("Order Details")]

    public string ClientName;

    //For more complexity we can add multiple delivery locations per order

    public PickupLocationSO[] PickupLocations;

    public DropLocationSO[] DropLocations;

    public string[] Items;

    [Header("Special Instructions")]

    //Maybe tool reqirements for some specific orders?

    public string ExtraNotes;

    [Header("Payment")]

    public int CashPayment;

    //Maybe some special rewards like special item or tools here?

    [HideInInspector] public DeliveryState CurrentDeliveryState = DeliveryState.PendingOrder;


    private void OnValidate()
    {
        #if UNITY_EDITOR
        OrderID = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif

    }
}
