using UnityEngine;

public class OrderGiver : MonoBehaviour
{
    [SerializeField] private OrderInfoSO _orderToGive;

        // Add to button later
    public void OnDeliveryAccepted()
    {
        DeliveryManager.Instance.AcceptOrder(_orderToGive);
    }
}