using UnityEngine;

public class PhysicalCargo : MonoBehaviour, IInteractable
{
    [Header("Data")]
    private LocationID _pickupLocID; 
    private LocationID _targetDropID;
    private Rigidbody _rb;
    private Collider _collider;

    private bool _isDelivered = false;
    public bool IsHeld { get; private set; }
    
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
        if (_isDelivered)
        {
            return;
        }

        bool canPickup = DeliveryManager.Instance.PickupPackage(_pickupLocID);
        if (!canPickup)
        {
            return; 
        }
        
        _rb.isKinematic = true; 
        _collider.isTrigger = true;
        transform.SetParent(interactor.GetPlayer().HandPos);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        if (interactor is PlayerInteractor playerInteractor)
        {
            playerInteractor.HoldCargo(this);
        }

        IsHeld = true;
    }

    public void DropFromHands()
    {
        transform.SetParent(null);
        _collider.isTrigger = false;
        _rb.isKinematic = false;
        IsHeld = false;
    }
    public void MarkAsDelivered()
    {
        _isDelivered = true;
        gameObject.tag = "Untagged"; 
    }
    public void SnapTo(Transform snapPoint)
    {
        _rb.isKinematic = true; 
        
        transform.SetParent(snapPoint);
        _collider.isTrigger = true;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity; 
    }
}