using UnityEngine;

/// <summary>
/// Controls the _rb of the player.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController _inputController;

    private PlayerState _playerState;

    [SerializeField] private Rigidbody _rb;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _groundDrag = 5f;
    [SerializeField] private float _slopeDrag = 7f;
    [SerializeField] private float _airMultiplier = 0.4f;

    private bool _canMove = true;

    [Header("Ground Check")]
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _orientation;

    private Vector3 _moveDirection;

    // Slope Handling
    private bool _isGrounded;
    private bool _isOnSlope;
    private RaycastHit _slopeHit;

    void Start()
    {
        _rb.freezeRotation = true;
        _rb.drag = _groundDrag;
    }

    void Update()
    {
        if (!_canMove)
        {
            return;
        }

        CheckGrounded();
        StateHandler();


        SpeedControl();

        if (_isOnSlope)
        {
            _rb.drag = _slopeDrag;
        }
        else
        {
            _rb.drag = _groundDrag;
        }
    }

    void FixedUpdate()
    {
        if (!_canMove)
            return;

        MovePlayer();
    }

    private void MovePlayer()
    {
        // Calculate move direction
        _moveDirection = _orientation.forward * _inputController.VerticalInput + _orientation.right * _inputController.HorizontalInput;
        // Slope
        if (_isOnSlope)
        {
            _moveDirection = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
            _rb.AddForce(-_slopeHit.normal * 80f, ForceMode.Force);
        }

        // Ground
        if (_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
        }
        else // In Air
        {
            _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * _airMultiplier), ForceMode.Force);
            _rb.AddForce(Vector3.down * 115f, ForceMode.Acceleration);
        }
    }


    private void SpeedControl()
    {
        if (_isOnSlope)
        {
            if (_rb.velocity.magnitude > _moveSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            if (flatVel.magnitude > _moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _moveSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }
        }
    }


    private void CheckGrounded()
    {
        // Ground Check
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundMask);

        // Slope Check
        _isOnSlope = false;
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            _isOnSlope = angle < 45 && angle != 0;
        }
    }


    public void SetMovementLock(bool isLocked)
    {
        _canMove = !isLocked;

        if (!_canMove)
        {
            _rb.velocity = Vector3.zero;
        }
    }

    private void StateHandler()
    {
        float speed = new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude;

        if (_playerState == PlayerState.Falling)
        {
        }

        if (_isGrounded)
        {
            if (speed > 0.1f)
                _playerState = PlayerState.Walking;
            else
                _playerState = PlayerState.Idle;
        }
        else
        {
            _playerState = PlayerState.Falling;
        }
    }


    #region DEBUG

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * (_playerHeight * 0.5f + 0.2f));
    }
#endif

    #endregion
}

public enum PlayerState
{
    Idle,
    Walking,
    Falling
}