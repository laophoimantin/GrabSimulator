using UnityEngine;

public class MotorcycleCollisionHandler : MonoBehaviour
{
    //[Header("Script References")]
    // Script References
    private MotorcycleSoundHandler soundHandler;
    private MotorbikeMovement motorbikeMovement;


    private void Awake()
    {
        soundHandler = this.transform.parent.GetComponent<MotorcycleSoundHandler>();
        motorbikeMovement = this.transform.parent.GetComponent<MotorbikeMovement>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            float impactSpeed = collision.relativeVelocity.magnitude;

            if (impactSpeed > Global.Motorcycle.MinimumCollisionForce)
            {
                soundHandler.CollisionSound(impactSpeed, motorbikeMovement.MaxSpeed);
            }

        }
    }
}
