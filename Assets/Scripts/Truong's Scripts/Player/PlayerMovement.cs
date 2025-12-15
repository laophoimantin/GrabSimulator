using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _groundDrag = 5f;
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private float _jumpCooldown = 0.25f;
    [SerializeField] private float _airMultiplier = 0.4f;
    [SerializeField] private bool _canMove = true;

    [Header("Key Bindings")]
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private LayerMask _groundMask;
    private bool _grounded;

    [SerializeField] private Transform _orientation;

    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _moveDirection;
    private Rigidbody _rb;
    private bool _readyToJump = true;

    // Slope Handling
    private RaycastHit _slopeHit;

    [Header("Gravity Tweaks")]
    [SerializeField] private float _fallMultiplier = 2.5f; // Gravity when falling
    [SerializeField] private float _lowJumpMultiplier = 2f; // Gravity when doing a short hop

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    void Update()
    {
        _grounded = Physics.SphereCast(transform.position, 0.4f, Vector3.down, out _, _playerHeight * 0.5f + 0.2f, _groundMask);

        MyInput();
        SpeedControl();

        // Drag handling
        if (_grounded)
            _rb.drag = _groundDrag;
        else
            _rb.drag = 0f;
    }

    void FixedUpdate()
    {
        if (_canMove)
        {
            MovePlayer();
        }

        ApplyGravityModifiers();
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_jumpKey))
        {
            if (_grounded && _readyToJump)
            {
                _readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), _jumpCooldown);
            }
        }
    }

    private void MovePlayer()
    {
        // Calculate move direction
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;

        // Slope Handling logic
        if (OnSlope())
        {
            _moveDirection = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
        }

        if (_grounded)
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        else
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * _airMultiplier, ForceMode.Force);

        _rb.useGravity = !OnSlope();
    }

    private void ApplyGravityModifiers()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (_rb.velocity.y > 0 && !Input.GetKey(_jumpKey))
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (_lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void SpeedControl()
    {
        if (OnSlope() && !_readyToJump)
        {
            if (_rb.velocity.magnitude > _moveSpeed)
                _rb.velocity = _rb.velocity.normalized * _moveSpeed;
        }
        else
        {
            Vector3 flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            if (flatVelocity.magnitude > _moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * _moveSpeed;
                _rb.velocity = new Vector3(limitedVelocity.x, _rb.velocity.y, limitedVelocity.z);
            }
        }
    }

    private void Jump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < 45 && angle != 0;
        }

        return false;
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
        if (!_canMove)
        {
            CursorManager.Instance.ShowCursor();
        }
        else
        {
            CursorManager.Instance.HideCursor();
        }
    }
}