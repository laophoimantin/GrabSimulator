using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupLocation : MonoBehaviour
{
    [SerializeField] private Transform _packageSpawnPoint;
    [SerializeField] private LocationNode _owner;
    private GameObject _currentPackage;

    public void SpawnPackage(GameObject packagePrefab, LocationID targetDropID)
    {
        if (_currentPackage != null)
        {
            Destroy(_currentPackage); 
        }

        _currentPackage = Instantiate(packagePrefab, _packageSpawnPoint.position, _packageSpawnPoint.rotation);
    
        PhysicalCargo cargo = _currentPackage.GetComponent<PhysicalCargo>();
        if (_owner == null)
        {
            Debug.LogError($"PickupLocation has no owner {name}");
        }
        if (cargo != null)
        {
            cargo.Initialize(_owner.ID, targetDropID);
        }
    }
}