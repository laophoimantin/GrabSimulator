using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControllerLegacy : MonoBehaviour 
{

    
    [Header("References")]
    [SerializeField] private Transform _modelTransform;
    

    [Header("Movement Settings")]
    [SerializeField] private float _acceleration = 50f; 
    [SerializeField] private float _maxSpeed = 20f;
    [SerializeField] private float _normalDrag = 2f;    // Ma sát bình thường
    [SerializeField] private float _brakeDrag = 10f;    // Ma sát khi phanh (Càng cao dừng càng nhanh)
    [SerializeField] private float _turnSpeed = 150f;   // Tốc độ xoay xe
    [SerializeField] private float _traction = 5f;      // Độ bám đường (Drift)

    [Header("Visual Tilt Settings")]
    [SerializeField] private float _maxTiltAngle = 10f; // Góc nghiêng tối đa khi cua (độ)
    [SerializeField] private float _tiltSpeed = 5f;     // Tốc độ nghiêng (mượt mà)

    // Variables
    private Vector3 _velocity;
    private float _moveInput;
    private float _steerInput;
    private bool _isBraking;
    
    private bool _canMove = true;

    void Update() 
    {
        if (!_canMove)
        {
            _velocity = Vector3.zero;
        }
        
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        GetInput();
        Move();
        Steer();
        VisualTilt();
    }

    private void GetInput()
    {
        if (!_canMove) return;
        _moveInput = Input.GetAxis("Vertical");
        _steerInput = Input.GetAxis("Horizontal");
        _isBraking = Input.GetKey(KeyCode.Space);
    }

    private void Move()
    {
        // 1. Tính toán gia tốc
        // Nếu đang phanh thì không cho tăng ga nữa
        if (!_isBraking)
        {
            _velocity += transform.forward * _moveInput * _acceleration * Time.deltaTime;
        }

        // 2. Xử lý Ma sát (Drag) & Phanh
        // Nếu bấm phanh -> dùng BrakeDrag, ngược lại dùng NormalDrag
        float currentDrag = _isBraking ? _brakeDrag : _normalDrag;
        
        // Lerp vận tốc về 0 dựa trên Drag. (Công thức ma sát giả lập)
        _velocity = Vector3.Lerp(_velocity, Vector3.zero, currentDrag * Time.deltaTime);

        // 3. Giới hạn tốc độ tối đa
        _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);

        // 4. Áp dụng vào vị trí (Di chuyển kiểu Legacy/Arcade)
        transform.position += _velocity * Time.deltaTime;

        // 5. Xử lý Traction (Drifting giả)
        // Kéo hướng di chuyển về hướng mũi xe đang nhìn
        if (_velocity.magnitude > 0.1f)
        {
            // Debug cho vui mắt
            Debug.DrawRay(transform.position, _velocity.normalized * 3, Color.red);
            Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);

            // Lerp vector vận tốc hiện tại sang hướng Forward của xe
            _velocity = Vector3.Lerp(_velocity.normalized, transform.forward, _traction * Time.deltaTime) * _velocity.magnitude;
        }

    }

    private void Steer()
    {
        // Chỉ cho phép bẻ lái khi xe đang di chuyển (Vận tốc > 0.1)
        if (_velocity.magnitude > 0.3f)
        {
            // Kiểm tra xem đang đi tiến hay lùi để đảo ngược hướng lái
            float direction = Vector3.Dot(_velocity, transform.forward) >= 0 ? 1f : -1f;
            float turnAmount = _steerInput * _turnSpeed * Time.deltaTime * direction;
            // Chỉ xoay quanh trục Y (Up). Trục Z và X bị khóa cứng theo logic này.
            transform.Rotate(0, turnAmount, 0);
        }
    }

    private void VisualTilt()
    {
        if (_modelTransform == null) return;

        // Tính góc nghiêng mục tiêu.
        // Cua trái (_steer < 0) -> Nghiêng sang phải (Z âm) hoặc ngược lại tùy Pivot model của ông.
        // Ở đây tôi để: Cua Trái -> Nghiêng trái (Z dương), Cua Phải -> Nghiêng phải (Z âm).
        // Chỉnh dấu trừ (-) trước _steerInput nếu muốn ngược lại.
        float targetZ = -_steerInput * _maxTiltAngle;

        // Nếu xe đứng im thì trả về thẳng đứng (0)
        if (_velocity.magnitude < 1f) targetZ = 0f;

        // Lerp góc Z hiện tại sang góc mục tiêu cho mượt
        float currentZ = _modelTransform.localEulerAngles.z;
        // Cần xử lý góc > 180 độ của Unity (ví dụ 350 độ = -10 độ)
        currentZ = (currentZ > 180) ? currentZ - 360 : currentZ;

        float newZ = Mathf.Lerp(currentZ, targetZ, _tiltSpeed * Time.deltaTime);

        // Áp dụng vào Model (Giữ nguyên X và Y local, chỉ thay đổi Z)
        _modelTransform.localEulerAngles = new Vector3(_modelTransform.localEulerAngles.x, _modelTransform.localEulerAngles.y, newZ);
    }

    public void SetCanMove(bool state)
    {
        _canMove = state;
    }
    
    
    
    
    // // Settings
    // public float MoveSpeed = 50;
    // public float MaxSpeed = 15;
    // public float Drag = 0.98f;
    // public float SteerAngle = 20;
    // public float Traction = 1;
    //
    // // Variables
    // private Vector3 _moveForce;
    //
    // // Update is called once per frame
    // void Update() {
    //
    //     // Moving
    //     _moveForce += transform.forward * MoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
    //     transform.position += _moveForce * Time.deltaTime;
    //
    //     // Steering
    //     float steerInput = Input.GetAxis("Horizontal");
    //     transform.Rotate(Vector3.up * steerInput * _moveForce.magnitude * SteerAngle * Time.deltaTime);
    //
    //     // Drag and max speed limit
    //     _moveForce *= Drag;
    //     _moveForce = Vector3.ClampMagnitude(_moveForce, MaxSpeed);
    //
    //     // Traction
    //     Debug.DrawRay(transform.position, _moveForce.normalized * 3);
    //     Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
    //     _moveForce = Vector3.Lerp(_moveForce.normalized, transform.forward, Traction * Time.deltaTime) * _moveForce.magnitude;
    // }
}