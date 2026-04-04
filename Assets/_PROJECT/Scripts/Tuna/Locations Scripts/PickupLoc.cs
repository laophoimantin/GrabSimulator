using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupLoc : MonoBehaviour
{
    [SerializeField] private LocationID _id;
    [SerializeField] private Transform _packageSpawnPoint;
    private GameObject _currentPackage;
    private bool HasPackage => _currentPackage != null;
    public static event Action<LocationID> OnPickupEntered;

    private void Start() 
    {
        DeliveryManager.Instance.RegisterPickup(_id, this);
    }

    private void OnDestroy()
    {
        if (DeliveryManager.Instance == null) return;
        DeliveryManager.Instance.UnregisterPickup(_id);
    }
    
    public void SpawnPackage(GameObject package)
    {
        _currentPackage = Instantiate(package, _packageSpawnPoint.position, _packageSpawnPoint.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !HasPackage)
            return;

        OnPickupEntered?.Invoke(_id);
    }

    public void RemovePackage()
    {
        if (_currentPackage != null)
        {
            Destroy(_currentPackage);
            _currentPackage = null;
        }
    }
}