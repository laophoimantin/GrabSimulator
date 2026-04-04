using UnityEngine;

public class BikeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rbSphere;

    private MotorbikeInput _input;
    private MotorbikePhysics _physics;
    private MotorbikeVisuals _visuals;

    private void Awake()
    {
        _input = GetComponent<MotorbikeInput>();
        _physics = GetComponent<MotorbikePhysics>();
        _visuals = GetComponent<MotorbikeVisuals>();

        HideDummyModel();
        
        LockMovement();
        LockBike();
        HideDummyModel();
    }
    public void LockMovement() => _input.LockMovement();
    public void UnlockMovement() => _input.UnlockMovement();

    public void HideDummyModel() => _visuals.SetDummyModelState(false);

    public void ShowDummyModel() => _visuals.SetDummyModelState(true);

    public void LockBike() => _physics.LockPhysics(true);
    public void UnlockBike() => _physics.LockPhysics(false);
}