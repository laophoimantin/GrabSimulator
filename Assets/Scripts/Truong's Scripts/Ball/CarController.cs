using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] private Rigidbody _rb;

    [Header("Settings")]
    [SerializeField] private float _acceleration = 50f;     
    [SerializeField] private float _maxSpeed = 20f;        
    [SerializeField] private float _turnSpeed = 150f;      
    [SerializeField] private float _driftFactor = 0.95f;   

    private float _moveInput;
    private float _steerInput;

    void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
    }

    void Update() 
    {
        _moveInput = Input.GetAxisRaw("Vertical");
        _steerInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
  
        if (_moveInput != 0)
        {
            _rb.AddForce(transform.forward * _moveInput * _acceleration, ForceMode.Acceleration);
        }


        float minSpeedToTurn = 1f;
        if (_rb.velocity.magnitude > minSpeedToTurn)
        {
            float direction = Vector3.Dot(_rb.velocity, transform.forward) > 0 ? 1 : -1;
            float turn = _steerInput * _turnSpeed * direction;
            _rb.AddTorque(Vector3.up * turn, ForceMode.Acceleration);
        }

        if (_rb.velocity.magnitude > _maxSpeed)
        {
            _rb.velocity = _rb.velocity.normalized * _maxSpeed;
        }

        ApplyTraction();
    }

    private void ApplyTraction()
    {
        Vector3 velocity = _rb.velocity;

        Vector3 forwardVelocity = transform.forward * Vector3.Dot(velocity, transform.forward);
        Vector3 rightVelocity = transform.right * Vector3.Dot(velocity, transform.right);
        Vector3 newRightVelocity = Vector3.Lerp(rightVelocity, Vector3.zero, _driftFactor * Time.fixedDeltaTime * 5f);

        _rb.velocity = forwardVelocity + newRightVelocity;
    }
    
    
    /*
     public class CarController : MonoBehaviour 
{

    [SerializeField] private Rigidbody _rb;
    // Settings
    [SerializeField] private float _moveSpeed = 50;
    [SerializeField] private float _maxSpeed = 15;
    [SerializeField] private float _drag = 0.98f;
    [SerializeField] private float _steerAngle = 20;
    [SerializeField] private float _traction = 1;

    // Variables
    private Vector3 _moveForce;
    private Vector3 _steerTorque;

    // Update is called once per frame
    void Update() {

        // Moving
        _moveForce += transform.forward * _moveSpeed * Input.GetAxis("Vertical");

        // Steering
        _steerTorque = Input.GetAxis("Horizontal") * Vector3.up * _steerAngle;
        _moveForce = Vector3.ClampMagnitude(_moveForce, _maxSpeed);

        // Traction
        Debug.DrawRay(transform.position, _moveForce.normalized * 3);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        _moveForce = Vector3.Lerp(_moveForce.normalized, transform.forward, _traction * Time.deltaTime) * _moveForce.magnitude;
    }

    private void FixedUpdate()
    {
        //transform.position += _moveForce * Time.deltaTime;

        _rb.AddForce(_moveForce, ForceMode.Force);
        _rb.AddTorque(_steerTorque);
    }
}

     */
}