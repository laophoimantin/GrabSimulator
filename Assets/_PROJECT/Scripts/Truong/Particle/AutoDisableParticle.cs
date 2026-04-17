using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDisableParticle : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
    }
}