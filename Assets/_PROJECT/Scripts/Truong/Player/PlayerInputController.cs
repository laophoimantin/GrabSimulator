using UnityEngine;
using UnityEngine.InputSystem; // Nhớ trò này không?

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveInput 
    { 
        get 
        {
            if (InputLocker.IsLocked(InputActionType.Move))
                return Vector2.zero; 
                
            return _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        }
    }

    public float HorizontalInput => MoveInput.x;
    public float VerticalInput => MoveInput.y;

    private InputAction _moveAction;

    private void Start()
    {
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            _moveAction = InputManager.Instance.InputActions.OnGround.Move;
        }
    }
}