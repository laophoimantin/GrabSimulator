using UnityEngine;

public class LocationNode : MonoBehaviour
{
    [Header("Location Config")]
    [SerializeField] private LocationID _id;
    [SerializeField] private AreaID _area;

    public LocationID ID => _id;
    public AreaID Area => _area;

    [Header("Pickup/Drop Locations")]
    [SerializeField] private PickupLocation _pickupLocation;
    [SerializeField] private DropLocation _dropLocation;
    
    private void Start()
    {
        DeliveryManager.Instance.RegisterStation(_id, this);
    }

    private void OnDestroy()
    {
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.UnregisterStation(_id);
        }
    }

    public void SpawnPackage()
    {
        Order currentOrder = DeliveryManager.Instance.GetCurrentOrder();
        if (currentOrder == null) return;

        GameObject cargoObj = currentOrder.CargoData.CargoPrefab;
        LocationID targetLocationID = currentOrder.DropLocID;
        _pickupLocation.SpawnPackage(cargoObj, targetLocationID);
    }
}