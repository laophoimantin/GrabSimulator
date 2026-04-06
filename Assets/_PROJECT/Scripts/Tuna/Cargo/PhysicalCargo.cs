using UnityEngine;

public class PhysicalCargo : MonoBehaviour, IInteractable
{
    [Header("Delivery Data")]
    private LocationID _pickupLocID; 
    private LocationID _targetDropID;
    
    private Rigidbody _rb;
    private Collider _collider;
    private string _cargoDefaultTag;

    private bool _isDelivered = false;
    public bool IsHeld { get; private set; }
    public LocationID TargetDropID => _targetDropID;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        
        _cargoDefaultTag = gameObject.tag; 
    }

    public void Initialize(LocationID pickup, LocationID drop)
    {
        _pickupLocID = pickup;
        _targetDropID = drop;
    }

    public void Interact(IInteractor interactor)
    {
        if (_isDelivered) return;

        bool canPickup = DeliveryManager.Instance.PickupPackage(_pickupLocID);
        if (!canPickup) return; 

        IsHeld = true;
        
        SetPhysicsState(isKinematic: true, isTrigger: true, newTag: "Untagged");

        AttachToTransform(interactor.GetPlayer().HandPos);

        if (interactor is PlayerInteractor playerInteractor)
        {
            SoundManager.Instance.PlaySound2D(DeliverySoundType.Item_Pick_Up, 0.15f);
            playerInteractor.HoldCargo(this);
        }
    }

    public void DropFromHands()
    {
        IsHeld = false;
        transform.SetParent(null);
        
        SetPhysicsState(isKinematic: false, isTrigger: false, newTag: _cargoDefaultTag);
    }

    public void SnapTo(Transform snapPoint)
    {
        SetPhysicsState(isKinematic: true, isTrigger: true, newTag: "Untagged");
        
        AttachToTransform(snapPoint);
    }

    public void MarkAsDelivered()
    {
        _isDelivered = true;
        gameObject.tag = "Untagged"; 
    }

    private void SetPhysicsState(bool isKinematic, bool isTrigger, string newTag)
    {
        _rb.isKinematic = isKinematic;
        _collider.isTrigger = isTrigger;
        gameObject.tag = newTag;
    }

    private void AttachToTransform(Transform target)
    {
        transform.SetParent(target);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

}