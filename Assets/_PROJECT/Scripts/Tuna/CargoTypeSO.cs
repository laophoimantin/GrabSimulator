using UnityEngine;

[CreateAssetMenu(fileName = "New Cargo Type", menuName = "Delivery/Cargo Type")]
public class CargoTypeSO : ScriptableObject
{
    public string CargoName;
    public GameObject CargoPrefab;

    [Tooltip("Idea range: 1 - 3")]
    public float DifficultyMultiplier = 1f;

    [Header("Unused")]
    public CargoType CargoType;
}

public enum CargoType
{
    Light,
    Medium,
    Heavy
}