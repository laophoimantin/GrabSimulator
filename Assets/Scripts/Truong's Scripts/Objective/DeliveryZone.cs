using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeliveryZone : MonoBehaviour
{
    [SerializeField] private LocationID _locationID;
    [SerializeField] private Transform _dropPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bike"))
        {
            //other.TryGetComponent(out PlayerInteractor player);
            DropOrder();
        }
    }

    private void DropOrder()
    {
        var orderManager = OrderManager.Instance;

        if (orderManager.CurrentState == OrderManager.OrderState.PackagePickedUp)
        {
            if (orderManager.CurrentOrder.DropoffLocationID == _locationID)
            {
                orderManager.DeliverPackage(_dropPoint);
            }
            else
            {
                Debug.Log("This isn't the customer's house.");
            }
        }
    }
}