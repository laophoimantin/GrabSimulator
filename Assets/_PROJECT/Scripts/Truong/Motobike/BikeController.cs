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
        
        LockBike();
        HideDummyModel();
    }

    public void HideDummyModel() => _visuals.SetDummyModelState(false);

    public void ShowDummyModel() => _visuals.SetDummyModelState(true);

    public void LockBike() => _physics.LockPhysics(true);
    public void UnlockPhysic() => _physics.LockPhysics(false);
}