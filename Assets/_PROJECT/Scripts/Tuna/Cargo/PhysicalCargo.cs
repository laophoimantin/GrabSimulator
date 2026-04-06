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

        IsHeld = true;
    }

    public void DropFromHands()
    {
        transform.SetParent(null);
        _collider.enabled = true;
        _rb.isKinematic = false;
        IsHeld = false;
        // _rb.AddForce(transform.forward * 2f, ForceMode.Impulse); 
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
        
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity; 
    }
}