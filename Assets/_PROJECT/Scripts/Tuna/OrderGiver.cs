using UnityEngine;

public class OrderGiver : MonoBehaviour
{
    [SerializeField] private OrderInfoSO _orderToGive;

    // Todo: Put this into a button
    public void OnDeliveryAccepted()
    {
        DeliveryManager.Instance.AcceptOrder(_orderToGive);
    }
}