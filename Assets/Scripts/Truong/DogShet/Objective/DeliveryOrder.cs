using UnityEngine;

[CreateAssetMenu(fileName = "New Order", menuName = "Delivery/Order")]
public class DeliveryOrder : ScriptableObject
{
    [Header("Info")]
    public string OrderName; // PIZZA
    public string Description;
    public int RewardMoney;

    [Header("Logistics")]
    public LocationID PickupLocationID;
    public LocationID DropoffLocationID;

    public GameObject PackagePrefab;
}