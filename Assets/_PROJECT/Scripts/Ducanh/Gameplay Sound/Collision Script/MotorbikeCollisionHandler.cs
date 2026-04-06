using UnityEngine;

public class MotorbikeCollisionHandler : MonoBehaviour
{
    [Header("Motorbike Script Reference")]
    [SerializeField] private MotorbikeSoundController soundController;
    [SerializeField] private MotorbikePhysics motorbikePhysics;


    private void Awake()
    {
        if (soundController == null) Debug.Log("Assign MotorbikeSoundController reference");
        if (motorbikePhysics == null) Debug.Log("Assign MotorbikePhysics reference");
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (soundController == null || motorbikePhysics == null) return;

        // 1. OBSTACLE COLLISION (Crashing into walls/cars)
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            float impactSpeed = collision.relativeVelocity.magnitude;

            if (impactSpeed > Global.Motorcycle.MinimumCollisionForce)
            {
                soundController.CollisionSound(impactSpeed, motorbikePhysics.MaxSpeed);
            }
        }

        // 2. GROUND COLLISION (Landing jumps)
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Vector3 contactNormal = collision.GetContact(0).normal;
            float upwardAlignment = Vector3.Dot(contactNormal, Vector3.up);

            if (upwardAlignment > 0.5f)
            {
                float compressionSpeed = Vector3.Dot(collision.relativeVelocity, contactNormal);
                float trueLandingForce = Mathf.Abs(compressionSpeed);

                if (trueLandingForce > Global.Motorcycle.MinimumLandingForce)
                {
                    soundController.LandingSound(trueLandingForce, Global.Motorcycle.MaximumLandingSpeed);
                }
            }
        }
    }
}
