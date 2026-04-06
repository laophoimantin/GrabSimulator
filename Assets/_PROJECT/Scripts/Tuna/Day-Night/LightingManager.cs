using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPresets Preset;
    private const float TIME_SPEED = 0.01f;

    [SerializeField, Range(0, 72)] private float TimeofDay;

    private void FixedUpdate()
    {
        if (!GameRule.TICK) { return; }

        TimeofDay += TIME_SPEED;

        if (TimeofDay >= 72f)
        {
            TimeofDay = 0f;
        }
        UpdateLighting(TimeofDay / 72f);
    }

    private void UpdateLighting(float TimePercent)
    {
        RenderSettings.ambientLight = Preset.ambientColor.Evaluate(TimePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(TimePercent);

        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.directionalColor.Evaluate(TimePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((TimePercent * 360f) - 90f, 170f, 0));
        }   
    }

    private void OnValidate()
    {
        if (DirectionalLight != null) return;

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (var light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }   
    }
}


