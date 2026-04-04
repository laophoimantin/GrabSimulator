using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LightingPresets", menuName = "Tuna/Day-Night/Lighting Presets", order = 1)]

public class LightingPresets : ScriptableObject
{
    public Gradient ambientColor;
    public Gradient directionalColor;
    public Gradient FogColor;
    public float intensity;
}
