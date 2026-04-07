using UnityEngine;

public class BikeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _bikeCam;

    [SerializeField]  private MotorbikeInput _input;
    [SerializeField]   private MotorbikePhysics _physics;
    [SerializeField]  private MotorbikeVisuals _visuals;
    [SerializeField] private FuelSystem _fuel;
    [SerializeField] private float _lowFuelSpeedMultiplier = 0.5f;
    
    private void Awake()
    {
        LockPhysics();
        HideDummyModel();
        _bikeCam.SetActive(false);
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

        if (Mathf.Abs(_input.ForwardInput) > 0.01f)
        {
            _fuel.ConsumeFuel(_fuel.GetConsumptionThisFrame() * Mathf.Abs(_input.ForwardInput));
        }
    }
    
    public void AcceptRider()
    {
        ShowDummyModel();
        UnlockPhysics(); 
        _bikeCam.SetActive(true);
        
        GameEvents.OnPlayerVehicleChanged?.Invoke(_fuel);
    }

    public void EjectRider()
    {
        LockPhysics(); 
        HideDummyModel();
        
        _bikeCam.SetActive(false);
        
        GameEvents.OnPlayerVehicleChanged?.Invoke(null); 
    }
    
    public void HideDummyModel() => _visuals.SetDummyModelState(false);

    public void ShowDummyModel() => _visuals.SetDummyModelState(true);

    public void LockPhysics() => _physics.LockPhysics(true);
    public void UnlockPhysics() => _physics.LockPhysics(false);
}