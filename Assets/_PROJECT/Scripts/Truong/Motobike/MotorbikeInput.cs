using UnityEngine;
using UnityEngine.InputSystem;

public class MotorbikeInput : MonoBehaviour
{
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
    public float MoveInput 
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

    public bool IsReversing => MoveInput < 0;

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