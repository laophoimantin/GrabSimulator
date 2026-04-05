using UnityEngine;

[CreateAssetMenu(fileName = "New Cargo Type", menuName = "Delivery/Cargo Type")]
public class CargoTypeSO :ScriptableObject
{
    public string CargoName;
    public CargoType CargoType;
    public GameObject CargoPrefab;
    public float DifficultyMultiplier = 1f;
}

public enum CargoType
{
    Light,
    Medium,
    Heavy
}
