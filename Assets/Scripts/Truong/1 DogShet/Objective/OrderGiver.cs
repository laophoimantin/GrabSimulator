using UnityEngine;

public class OrderGiver : MonoBehaviour, IInteractable
{
    [SerializeField] private DeliveryOrder _orderToGive;
    [SerializeField] private PackagePickupZone _pickupZone;

    public void Interact(IInteractor interactor)
    {
        //ProcessOrder();
    }

    public void ProcessOrder()
    {
        if (OrderManager.Instance.CurrentState == OrderManager.OrderState.NoOrder)
        {
            OrderManager.Instance.AcceptOrder(_orderToGive);
            _pickupZone.SpawnPackage(_orderToGive.PackagePrefab);
        }
    }
}