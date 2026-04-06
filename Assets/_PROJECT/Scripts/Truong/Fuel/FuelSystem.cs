using UnityEngine;

public class FuelSystem : MonoBehaviour
{
    [SerializeField] private float _maxFuel = 100f;
    [SerializeField] private float _fuelConsumptionRate = 10f;

    private float _currentFuel;
    public bool IsOutOfFuel => _currentFuel <= 0f;
    public float FuelPercent => _currentFuel / _maxFuel;
    public bool IsFull => _currentFuel >= _maxFuel;

    
    private void Awake()
    {
        _currentFuel = _maxFuel;
    }

    public void ConsumeFuel(float amount)
    {
        if (IsOutOfFuel) return;
        _currentFuel = Mathf.Max(0f, _currentFuel - amount);
    }

    public float GetConsumptionThisFrame()
    {
        return _fuelConsumptionRate * Time.fixedDeltaTime;
    }
    

    public void Refuel()
    {
        _currentFuel = _maxFuel;
    }
}