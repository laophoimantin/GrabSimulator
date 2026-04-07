using UnityEngine;

public class FuelStation : MonoBehaviour
{
    [SerializeField] private int _refuelCost = 50;
    private FuelSystem _bikeInPumpZone;
    
    private void OnTriggerEnter(Collider other)
    {
        BikeProxy proxy = other.GetComponent<BikeProxy>();
        if (proxy != null && proxy.FuelSystem != null)
        {
            _bikeInPumpZone = proxy.FuelSystem;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BikeProxy proxy = other.GetComponent<BikeProxy>();
        if (proxy != null && proxy.FuelSystem != null)
        {
            if (_bikeInPumpZone == proxy.FuelSystem)
            {
                _bikeInPumpZone = null;
            }
        }
    }
    
    public RefuelResult TryRefuel()
    {
        if (_bikeInPumpZone == null) 
            return RefuelResult.NoBikeInZone;

        if (_bikeInPumpZone.FuelPercent >= 90) 
            return RefuelResult.TankAlreadyFull;

        if (!WalletSystem.Instance.TrySpend(_refuelCost))
            return RefuelResult.NotEnoughMoney;

        _bikeInPumpZone.Refuel();
        return RefuelResult.Success;
    }
    
    
}
public enum RefuelResult
{
    Success,
    TankAlreadyFull,
    NotEnoughMoney,
    NoBikeInZone
}