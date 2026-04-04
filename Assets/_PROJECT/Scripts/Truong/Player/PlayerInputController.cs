using UnityEngine;
using UnityEngine.InputSystem; // Nhớ trò này không?

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

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

    void Update()
    {
        if (_moveAction != null)
        {
            MoveInput = _moveAction.ReadValue<Vector2>();
        }
    }
}