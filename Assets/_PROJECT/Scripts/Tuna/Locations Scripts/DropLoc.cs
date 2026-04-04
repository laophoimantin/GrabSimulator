using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DropLoc : MonoBehaviour
{
    [SerializeField] private LocationID _id;
    public static event Action<LocationID> OnDropEntered;

    private void Start()
    {
        DeliveryManager.Instance.RegisterDrop(_id, this);
    }

    private void OnDestroy()
    {
        if (DeliveryManager.Instance == null) return;
        DeliveryManager.Instance.UnregisterDrop(_id);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Drop();
        }
    }

    private void Drop()
    {
        OnDropEntered?.Invoke(_id);
    }
}