using UnityEngine;
using UnityEngine.InputSystem;

public class MotorbikeInput : MonoBehaviour
{
	public bool IsHonking { get; private set; }

	public bool CanControl { get; private set; } = true;

    private InputAction _moveAction;
    private InputAction _brakeAction;

    private void Start()
    {
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            var onBikeMap = InputManager.Instance.InputActions.OnBike;

            _moveAction = onBikeMap.Move;
            _brakeAction = onBikeMap.Brake;
        }
    }

    private void OnEnable()
    {
        if (InputManager.Instance == null)
            return;

        InputManager.Instance.InputActions.OnBike.Honk.started += OnHonkStarted;
        InputManager.Instance.InputActions.OnBike.Honk.canceled += OnHonkCancelled;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null)
            return;

        InputManager.Instance.InputActions.OnBike.Honk.started -= OnHonkStarted;
        InputManager.Instance.InputActions.OnBike.Honk.canceled -= OnHonkCancelled;
    }

    private void OnHonkStarted(InputAction.CallbackContext ctx) => IsHonking = true;
    private void OnHonkCancelled(InputAction.CallbackContext ctx) => IsHonking = false;

    public float ForwardInput 
    {
        get 
        {
            if (InputLocker.IsLocked(InputActionType.BikeMove))
            {
                return 0f;
            }
            return _moveAction != null ? _moveAction.ReadValue<Vector2>().y : 0f;
        }
    }

    public float SteerInput 
    {
        get 
        {
            if (InputLocker.IsLocked(InputActionType.BikeMove)) 
            {
                return 0f;
            }
            return _moveAction != null ? _moveAction.ReadValue<Vector2>().x : 0f;
        }
    }


    public bool IsReversing => ForwardInput < 0;

    public bool IsBraking 
    {
        get 
        {
            if (InputLocker.IsLocked(InputActionType.BikeBrake))  {
                return false;
            }
            return _brakeAction != null && _brakeAction.IsPressed();
        }
    }
}