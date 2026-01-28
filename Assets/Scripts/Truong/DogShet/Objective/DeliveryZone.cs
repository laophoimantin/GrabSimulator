using System;
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

    void Start()
    {
        OrderManager.Instance.RegisterDropoff(_locationID, transform);
    }
    
    private void DropOrder()
    {
        var orderManager = OrderManager.Instance;

        if (orderManager.CurrentState == OrderManager.OrderState.PackagePickedUp)
        {
            if (orderManager.CurrentOrder.DropoffLocationID == _locationID)
            {
                orderManager.DeliverPackage(_dropPoint);
                PlayEffect();

            }
            else
            {
                Debug.Log("This isn't the customer's house.");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            PlayEffect();
        }
    }

    private void PlayEffect()
    {
        EffectManager.Instance.PlayVFX("Firework", transform.position, Quaternion.LookRotation(Vector3.up));
    }
}
