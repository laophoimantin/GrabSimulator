using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DropLocation : MonoBehaviour
{
    [SerializeField] private LocationNode _owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) return;
        
        PhysicalCargo cargo = other.GetComponent<PhysicalCargo>();
        if (cargo != null)
        {
            if (cargo.TargetDropID == _owner.ID)
            {
                bool isDelivered = DeliveryManager.Instance.DeliverPackage(_owner.ID);
                
                if (isDelivered)
                {
                    cargo.MarkAsDelivered();
                    Destroy(cargo.gameObject, 10);
                }
            }
        }
    }
}