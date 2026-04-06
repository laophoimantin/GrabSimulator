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

        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            float impactSpeed = collision.relativeVelocity.magnitude;

            if (impactSpeed > Global.Motorcycle.MinimumCollisionForce)
            {
                soundController.CollisionSound(impactSpeed, motorbikePhysics.MaxSpeed);
            }
        }
    }
}
