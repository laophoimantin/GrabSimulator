using UnityEngine;
using UnityEngine.InputSystem;

public class MotorbikeInput : MonoBehaviour
{
    public float MoveInput { get; private set; }
    public bool IsReversing => MoveInput < 0;
    public float SteerInput { get; private set; }
    public bool IsBraking { get; private set; }

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

            _brakeAction.started += OnBrakeStarted;
            _brakeAction.canceled += OnBrakeCanceled;
        }
        else
        {
            Debug.LogError("Mất tích InputManager! Check lại Script Execution Order đi.");
        }
    }

    private void OnDestroy()
    {
        if (_brakeAction != null)
        {
            _brakeAction.started -= OnBrakeStarted;
            _brakeAction.canceled -= OnBrakeCanceled;
        }
    }

    private void OnBrakeStarted(InputAction.CallbackContext ctx) => IsBraking = true;
    private void OnBrakeCanceled(InputAction.CallbackContext ctx) => IsBraking = false;

    void Update()
    {
        if (!CanControl)
        {
            MoveInput = 0f;
            SteerInput = 0f;
            IsBraking = false;
            return;
        }

        if (_moveAction != null)
        {
            Vector2 inputDir = _moveAction.ReadValue<Vector2>();
            MoveInput = inputDir.y;
            SteerInput = inputDir.x;
        }
    }
    
    public void LockMovement() => SetMovementLock(true);
    public void UnlockMovement() => SetMovementLock(false);
    
    private void SetMovementLock(bool isLocked)
    {
        CanControl = !isLocked;

        if (isLocked)
        {
            IsBraking = false;
        }
    }
}