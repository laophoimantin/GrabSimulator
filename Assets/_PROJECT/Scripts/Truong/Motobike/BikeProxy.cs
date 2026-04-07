using UnityEngine;

public class BikeProxy : MonoBehaviour
{
    [SerializeField] private FuelSystem _mainFuelSystem;
    public FuelSystem FuelSystem => _mainFuelSystem;
}