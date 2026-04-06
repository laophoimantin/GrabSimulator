using UnityEngine;

public class BikeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rbSphere;

    private MotorbikeInput _input;
    private MotorbikePhysics _physics;
    private MotorbikeVisuals _visuals;
    private FuelSystem _fuel;
    public FuelSystem FuelSystem => _fuel;
    [SerializeField] private float _lowFuelSpeedMultiplier = 0.5f;
    
    private void Awake()
    {
        _input = GetComponent<MotorbikeInput>();
        _physics = GetComponent<MotorbikePhysics>();
        _visuals = GetComponent<MotorbikeVisuals>();
        _fuel = GetComponent<FuelSystem>();

        HideDummyModel();
        
        LockPhysics();
        HideDummyModel();
    }

    private void FixedUpdate()
    {
        if (_fuel.IsOutOfFuel)
        {
            _physics.PowerMultiplier = _lowFuelSpeedMultiplier;
        }
        else
        {
            _physics.PowerMultiplier = 1f;
        }

        if (Mathf.Abs(_input.MoveInput) > 0.01f)
        {
            _fuel.ConsumeFuel(_fuel.GetConsumptionThisFrame() * Mathf.Abs(_input.MoveInput));
        }
    }
    
    
    
    public void HideDummyModel() => _visuals.SetDummyModelState(false);

    public void ShowDummyModel() => _visuals.SetDummyModelState(true);

    public void LockPhysics() => _physics.LockPhysics(true);
    public void UnlockPhysics() => _physics.LockPhysics(false);
}