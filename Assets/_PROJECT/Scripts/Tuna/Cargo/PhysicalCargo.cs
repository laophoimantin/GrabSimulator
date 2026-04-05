using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PhysicalCargo : MonoBehaviour, IInteractable
{
    [Header("Data")]
    private LocationID _pickupLocID; 
    private LocationID _targetDropID;
    private Rigidbody _rb;

    public LocationID TargetDropID => _targetDropID;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Initialize(LocationID pickup, LocationID drop)
    {
        _pickupLocID = pickup;
        _targetDropID = drop;
    }

    public void Interact(IInteractor interactor)
    {
        DeliveryManager.Instance.PickupPackage(_pickupLocID);

        _rb.isKinematic = true; 
        transform.SetParent(interactor.GetPlayer().transform);
        transform.localPosition = Vector3.zero;
    }

    public void DropFromHands()
    {
        _rb.isKinematic = false;
        transform.SetParent(null);
    }
}