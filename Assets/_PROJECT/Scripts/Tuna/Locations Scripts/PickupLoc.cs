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

    private void Awake()
    {
        DeliveryManager.Instance.Register(_id, this);
    }

    public void SpawnPackage(GameObject package)
    {
        _currentPackage = Instantiate(package, _packageSpawnPoint.position, _packageSpawnPoint.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!HasPackage)
            return;

        Pickup();
    }

    private void Pickup()
    {
        if (_currentPackage != null)
        {
            OnPickupEntered?.Invoke(_id);
            Destroy(_currentPackage);
            _currentPackage = null;
        }
    }
}