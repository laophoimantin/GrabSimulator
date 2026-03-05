using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupLoc : MonoBehaviour
{
    [SerializeField] private LocationID _id;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DeliveryManager.Instance.HasOrder &&
                DeliveryManager.Instance.DeliveryStates.CurrentDeliveryState == DeliveryState.AcceptedOrder)
            {
                // Check if this pickup location matches the current order's pickup location
                if (DeliveryManager.Instance.CurrentOrder.PickupLocation == _id)
                {
                    Debug.Log("Package picked up successfully!");
                    DeliveryManager.Instance.PickupPackage();
                }
                else
                {
                    Debug.Log("This is not the correct pickup location for the current order.");
                }
            }
        }
    }
}
