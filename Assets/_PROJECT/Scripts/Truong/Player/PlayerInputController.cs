using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private InputAction _moveAction;

    private void Start()
    {
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            var onGroundMap = InputManager.Instance.InputActions.OnGround;
            _moveAction = onGroundMap.Move;
        }
    }
    public Vector2 MoveInput 
    { 
        get 
        {
            if (InputLocker.IsLocked(InputActionType.OnGroundMove))
                return Vector2.zero; 
                
            return _moveAction != null ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
        }
    }

    public float HorizontalInput => MoveInput.x;
    public float VerticalInput => MoveInput.y;

}