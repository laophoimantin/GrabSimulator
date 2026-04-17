using UnityEngine;

public class FuelStation : MonoBehaviour
{
    [SerializeField] private float _refuelCost = 30000f;
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

        if (_bikeInPumpZone.FuelPercent >= 0.9f)
        {
            return RefuelResult.TankAlreadyFull;
        }

        float missingFuelPercent = 1f - _bikeInPumpZone.FuelPercent;
        float rawCost = missingFuelPercent * _refuelCost;
        int finalCost = (Mathf.RoundToInt(rawCost + 250) / 500) * 500;
        finalCost = Mathf.Max(finalCost, 500);

        if (!WalletSystem.Instance.TrySpend(finalCost))
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