using UnityEngine;

public class PhysicalCargo : MonoBehaviour, IInteractable
{
    [Header("Data")]
    private LocationID _pickupLocID; 
    private LocationID _targetDropID;
    private Rigidbody _rb;
    private Collider _collider;

    public LocationID TargetDropID => _targetDropID;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
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
        _collider.enabled = false;
        
        transform.SetParent(interactor.GetPlayer().HandPos);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        if (interactor is PlayerInteractor playerInteractor)
        {
            playerInteractor.HoldCargo(this);
        }
    }

    public void DropFromHands()
    {
        transform.SetParent(null);
        _collider.enabled = true;
        _rb.isKinematic = false;
        
        // _rb.AddForce(transform.forward * 2f, ForceMode.Impulse); 
    }
}