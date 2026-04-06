using UnityEngine;

public class MotorcycleRack : MonoBehaviour
{
    [SerializeField] private Transform _snapPoint; 
    private PhysicalCargo _currentCargo; 

    private void OnTriggerStay(Collider other)
    {
        if (_currentCargo != null) return;

        PhysicalCargo cargo = other.GetComponent<PhysicalCargo>();
        
        if (cargo != null)
        {
            if (!cargo.IsHeld)
            {
                cargo.SnapTo(_snapPoint);
                
                _currentCargo = cargo; 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_currentCargo != null && other.gameObject == _currentCargo.gameObject)
        {
            _currentCargo = null;
        }
    }
}