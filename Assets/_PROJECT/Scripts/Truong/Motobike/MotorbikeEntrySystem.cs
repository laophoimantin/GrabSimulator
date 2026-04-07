using UnityEngine;
using UnityEngine.InputSystem;

public class MotorbikeEntrySystem : MonoBehaviour, IInteractable
{
    private PlayerController _driver;
    private VehicleState _state = VehicleState.Empty;
    public VehicleState State { get { return _state; } }

    [Header("References")]

    [SerializeField] private BikeController _controller;
    [SerializeField] private Transform _exitPoint;

    [Header("Motorbike Sound Controller")]

    [SerializeField] private MotorbikeSoundController _soundController;

    private void OnEnable()
    {
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            InputManager.Instance.InputActions.OnBike.ExitBike.performed += OnExitInput;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            InputManager.Instance.InputActions.OnBike.ExitBike.performed -= OnExitInput;
        }
    }

    private void OnExitInput(InputAction.CallbackContext context)
    {
        if (_state == VehicleState.Occupied)
        {
            _soundController?.DisengageEngineSound();
            ExitVehicle();
        }
    }

    public void Interact(IInteractor interactor)
    {
        if (_state != VehicleState.Empty)
            return;

        EnterVehicle(interactor.GetPlayer());
    }

    private void EnterVehicle(PlayerController driver)
    {
        _state = VehicleState.Occupied;
        InputManager.Instance.SetMotorcycleInputState();
            
        _driver = driver;
        _driver.MountVehicle(_exitPoint);
        
        _controller.AcceptRider();
    }

    private void ExitVehicle()
    {
        if (_state != VehicleState.Occupied)
            return;

        _state = VehicleState.Empty;
        InputManager.Instance.SetPlayerInputState();

        _controller.EjectRider();
        
        _driver.DismountVehicle(_exitPoint);
        _driver = null;
    }
}

public enum VehicleState
{
    Empty,
    Occupied
}