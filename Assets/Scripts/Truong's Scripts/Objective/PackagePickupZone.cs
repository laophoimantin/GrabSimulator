using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PackagePickupZone : MonoBehaviour
{
    [SerializeField] private LocationID _locationID;
    [SerializeField] private Transform _packageSpawnPoint;
    private GameObject _currentPackage;

    void Start()
    {
        OrderManager.Instance.RegisterPickup(_locationID, transform);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bike"))
        {
            //other.TryGetComponent(out PlayerInteractor player);
            PickupOrder();
        }
    }

    public void SpawnPackage(GameObject package)
    {
        _currentPackage = Instantiate(package, transform.position, Quaternion.identity);
    }
    
    private void PickupOrder()
    {
        var orderManager = OrderManager.Instance;

        if (orderManager.CurrentState != OrderManager.OrderState.Accepted) return;

        if (orderManager.CurrentOrder.PickupLocationID == _locationID)
        {
            orderManager.PickupPackage(_currentPackage);
        }
        else
        {
            Debug.Log("Wrong store.");
        }
    }
}