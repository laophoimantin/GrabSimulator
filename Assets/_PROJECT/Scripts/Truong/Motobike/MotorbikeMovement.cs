
using UnityEngine;
public class MotorbikeMovement : MonoBehaviour
{

    private bool _canControl = false;
    
    RaycastHit _hit;

    private float _moveInput;
    private float _steerInput;

    [Header("References")]
    [SerializeField] private Rigidbody _rbSphere;
    [SerializeField] private Rigidbody _rbBikeBody;

    [Header("Movement Settings")]
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _steerStrength = 10f;
    
    [Header("Breaking Settings")]
    [Range(0, 10)]
    [SerializeField] private float _breakingFactor = 0.5f;
    [SerializeField] private KeyCode _breakKey = KeyCode.Space;
    
    [SerializeField] float _bikeXTiltIncrement = 0.11f;
    [SerializeField] private float _zTiltAngle = 60f;

    [Header("Handle")]
    [SerializeField] private GameObject _handle;
    [SerializeField] private float _handleRotVal = 30f;
    [SerializeField] private float _handleRotSpeed = 0.15f;

    private float _currentVelocityOffset;
    private Vector3 _velocity;

    [Header("Ground Check")]
    private float _rayLength;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Gravity Settings")]
    [SerializeField] private float _gravity;

    [Header("Skid Marks")]
    [SerializeField] private TrailRenderer _skidMarks;
    [SerializeField] private float _shidWidth = 0.062f;
    [SerializeField] private float _minSkidVelocity = 10f;

    [Header("Sound")]
    [SerializeField] AudioSource _engineSound;
    [SerializeField] AudioSource _skidSound;
    [Range(0, 1)]
    [SerializeField] private float _minPitch = 0.5f;
    [Range(1, 5)]
    [SerializeField] private float _maxPitch = 0.5f;

    [Header("Wheels")]
    [SerializeField] private Transform _frontWheel;
    [SerializeField] private Transform _rearWheel;
    [SerializeField] private float _wheelRotSpeed = 1000f;

    [Header("Drift")]
    [SerializeField] private float _norDrag = 2f;
    [SerializeField] private float _driftDrag = 0.5f;

    [SerializeField] private AnimationCurve _turningCurve;
    
    private void Start()
    {
        _rbSphere.transform.parent = null;
        _rbBikeBody.transform.parent = null;

        _rayLength = _rbSphere.GetComponent<SphereCollider>().radius + 0.2f;
        _skidMarks.startWidth = _shidWidth;
        _skidMarks.emitting = false;

        _skidSound.mute = true;
    }

    void Update()
    {
        if (!_canControl)
            return;

        _moveInput = InputManager.Instance.InputActions.OnBike.Move.ReadValue<Vector2>().y;
        _steerInput = InputManager.Instance.InputActions.OnBike.Move.ReadValue<Vector2>().x;

        transform.position = _rbSphere.transform.position;

        _velocity = _rbBikeBody.transform.InverseTransformDirection(_rbBikeBody.velocity);
        _currentVelocityOffset = _velocity.z / _maxSpeed;
    }

    private void FixedUpdate()
    {
        if (!_canControl)
            return;
        
        Movement();

        // Visuals
        SkidMarks();
        _frontWheel.Rotate(Vector3.forward, -_wheelRotSpeed * Time.deltaTime * _currentVelocityOffset);
        _rearWheel.Rotate(Vector3.forward, -_wheelRotSpeed * Time.deltaTime * _moveInput);

        // Sounds
        EngineSound();
    }

    void Movement()
    {
        if (Grounded())
        {
            if (!Input.GetKey(_breakKey))
            {
                Acceleration();
            }
            Rotation();

            Break();
        }
        else
        {
            Gravity();
        }

        BikeTilt();
    }

    private void Acceleration()
    {
        _rbSphere.velocity = Vector3.Lerp(_rbSphere.velocity, _moveInput * _maxSpeed * transform.forward, _acceleration * Time.fixedDeltaTime);
    }

    private void Rotation()
    {
        transform.Rotate(0, _steerInput * _moveInput * _turningCurve.Evaluate(Mathf.Abs(_currentVelocityOffset)) * _steerStrength * Time.deltaTime, 0, Space.World);

        // Visuals
        _handle.transform.localRotation = Quaternion.Slerp(_handle.transform.localRotation,
            Quaternion.Euler(_handle.transform.localRotation.eulerAngles.x, _handleRotVal * _steerInput, _handle.transform.localRotation.eulerAngles.z), _handleRotSpeed);
    }

    private void BikeTilt()
    {
        float xRot = (Quaternion.FromToRotation(_rbBikeBody.transform.up, _hit.normal) * _rbBikeBody.transform.rotation).eulerAngles.x;
        float zRot = 0;

        if (_currentVelocityOffset > 0)
        {
            zRot = -_zTiltAngle * _steerInput * _currentVelocityOffset;
        }

        Quaternion targetRot = Quaternion.Slerp(_rbBikeBody.transform.rotation, Quaternion.Euler(xRot, transform.eulerAngles.y, zRot), _bikeXTiltIncrement);

        Quaternion newRotation = Quaternion.Euler(targetRot.eulerAngles.x, transform.eulerAngles.y, targetRot.eulerAngles.z);

        _rbBikeBody.MoveRotation(newRotation);
    }

    private void Break()
    {
        if (Input.GetKey(_breakKey))
        {
            _rbSphere.velocity *= _breakingFactor / 10;
            _rbSphere.drag *= _driftDrag;
        }
        else
        {
            _rbSphere.drag = _norDrag;
        }
    }

    private bool Grounded()
    {
        float radius = _rayLength - 0.02f;
        Vector3 origin = _rbSphere.position + radius * Vector3.up;
            
        if (Physics.SphereCast(origin, radius + 0.02f, -transform.up, out _hit, _rayLength, _groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Gravity()
    {
        _rbSphere.AddForce(_gravity * Vector3.down, ForceMode.Acceleration);
    }

    private void SkidMarks()
    {
        if (Grounded() && Mathf.Abs(_velocity.x) > _minSkidVelocity || Input.GetKey(_breakKey))
        {
            _skidMarks.emitting = true;

            _skidSound.mute = false;
        }
        else
        {
            _skidMarks.emitting = false;

            _skidSound.mute = true;
        }
    }

    private void EngineSound()
    {
        _engineSound.pitch = Mathf.Lerp(_minPitch, _maxPitch, Mathf.Abs(_currentVelocityOffset));
    }
    
    public void SetMovementLock(bool isLocked)
    {
        _canControl = !isLocked;

        if (!_canControl)
        {
            _rbSphere.velocity = Vector3.zero;
        }
    }
}