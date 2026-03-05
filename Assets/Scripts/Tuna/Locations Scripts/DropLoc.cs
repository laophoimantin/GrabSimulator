using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DropLoc : MonoBehaviour
{
    [SerializeField] private LocationID _id;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DeliveryManager.Instance.HasOrder &&
                DeliveryManager.Instance.DeliveryStates.CurrentDeliveryState == DeliveryState.PackagePickedUp)
            {
                // Check if this drop location matches the current order's drop location
                if (DeliveryManager.Instance.CurrentOrder.DropLocation == _id)
                {
                    Debug.Log("Package delivered successfully!");
                    DeliveryManager.Instance.DeliverPackage();
                }
                else
                {
                    Debug.Log("This is not the correct drop location for the current order.");
                }
            }
        }
    }
}
