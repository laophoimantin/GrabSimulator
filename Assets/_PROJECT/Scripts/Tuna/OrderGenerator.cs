using System.Collections.Generic;
using UnityEngine;

public class OrderGenerator : Singleton<OrderGenerator>
{
    [Header("Data Config")]
    [SerializeField] private List<CargoTypeSO> _availableCargos;
    [SerializeField] private List<LocationID> _allLocations;

    [Header("Economy Config")]
    [SerializeField] private float _baseRatePerMeter = 10f;
    [SerializeField] private int _minTip = 0;
    [SerializeField] private int _maxTip = 50;

    public Order GenerateRandomOrder()
    {
        List<LocationID> validPickups = DeliveryManager.Instance.GetAvailablePickupLocations();
        List<LocationID> validDrops = DeliveryManager.Instance.GetAvailableDropLocations();

        if (validPickups.Count == 0 || validDrops.Count == 0)
            return null;

        if (_availableCargos.Count == 0)
            return null;

        LocationID pickup = validPickups[Random.Range(0, validPickups.Count)];
        LocationID drop;

        int antiInfiniteLoop = 0;
        do
        {
            drop = validDrops[Random.Range(0, validDrops.Count)];
            antiInfiniteLoop++;
            if (antiInfiniteLoop > 100) 
                return null;
        } while (drop == pickup);

        CargoTypeSO randomCargo = _availableCargos[Random.Range(0, _availableCargos.Count)];

        Vector3 posA = DeliveryManager.Instance.GetLocationPosition(pickup);
        Vector3 posB = DeliveryManager.Instance.GetLocationPosition(drop);
        float dist = CalculateDistance(posA, posB);
        int reward = CalculateReward(dist, randomCargo.DifficultyMultiplier);

        return new Order
        {
            // System.Guid.NewGuid()
            // => Creates a new globally unique identifier (GUID), like: d2f1c7b4-8c2a-4d1a-9e2f-7b3a1c0f5e9a
            //.Substring(0, 8) 
            // => Takes only the first 8 characters of that string
            
            OrderID = System.Guid.NewGuid().ToString().Substring(0, 8),
            CargoData = randomCargo,
            PickupLocID = pickup,
            DropLocID = drop,
            Distance = dist,
            Reward = reward
        };
    }

    // =====================================================
    private int CalculateReward(float distance, float difficultyMultiplier)
    {
        float baseReward = distance * _baseRatePerMeter;
        float difficultyAdjustedReward = baseReward * difficultyMultiplier;

        int tip = Random.Range(_minTip, _maxTip);

        int finalReward = Mathf.RoundToInt(difficultyAdjustedReward) + tip;

        return Mathf.Max(finalReward, 10);
    }

    private float CalculateDistance(Vector3 startPos, Vector3 endPos)
    {
        // TODO: Later

        return Random.Range(100f, 1000f);
    }
}