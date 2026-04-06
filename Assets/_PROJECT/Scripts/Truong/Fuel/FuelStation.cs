using UnityEngine;

public class FuelStation : MonoBehaviour
{
    [SerializeField] private int _refuelCost = 50;
    [SerializeField] private FuelSystem _fuelSystem;


    public void TryRefuel()
    {
        if (_fuelSystem.IsFull) return;

        if (!WalletSystem.Instance.TrySpend(_refuelCost))
        {
            Debug.Log("Not enough money!");
            return;
        }

        _fuelSystem.Refuel();
    }
}