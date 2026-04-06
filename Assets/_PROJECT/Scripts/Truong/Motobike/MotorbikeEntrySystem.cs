using UnityEngine;
using UnityEngine.InputSystem;

public class MotorbikeEntrySystem : MonoBehaviour, IInteractable
{
    private PlayerController _driver;
    private VehicleState _state = VehicleState.Empty;
    public VehicleState State { get { return _state; } }

    [Header("References")]

    [SerializeField] private BikeController _controller;
    [SerializeField] private GameObject _bikeCam;
    [SerializeField] private Transform _exitPoint;

    [Header("Motorbike Sound Controller")]

    [SerializeField] private MotorbikeSoundController _soundController;

    void Awake()
    {
        _bikeCam.SetActive(false);
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            InputManager.Instance.InputActions.OnBike.Interact.performed += OnExitInput;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            InputManager.Instance.InputActions.OnBike.Interact.performed -= OnExitInput;
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

        _driver.transform.SetParent(_controller.transform);
        _driver.HideModel();
        _controller.ShowDummyModel();
        
        
        _controller.UnlockPhysic();
        
        _bikeCam.SetActive(true);
    }

    private void ExitVehicle()
    {
        if (_state != VehicleState.Occupied)
            return;

        _state = VehicleState.Empty;
        InputManager.Instance.SetPlayerInputState();

        _controller.UnlockPhysic();

        _controller.HideDummyModel();
        _driver.ShowModel();
        
        _driver.transform.position = _exitPoint.position;
        _driver.transform.rotation = _exitPoint.rotation;
        _driver.transform.SetParent(null);
        _driver = null;

        _bikeCam.SetActive(false);
    }
}

public enum VehicleState
{
    Empty,
    Occupied
}