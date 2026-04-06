using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ModernCarController : MonoBehaviour
{
    [Header("Cấu hình Động cơ")]
    [SerializeField] private float _acceleration = 30f;      // Tăng tốc
    [SerializeField] private float _maxSpeed = 20f;          // Tốc độ tối đa
    [SerializeField] private float _reverseSpeed = 10f;      // Tốc độ lùi
    [SerializeField] private float _turnSpeed = 100f;        // Tốc độ bẻ lái
    
    [Header("Cấu hình Drift & Vật lý")]
    [SerializeField] private float _groundDrag = 3f;         // Ma sát khi chạm đất (Cao = Dừng nhanh)
    [SerializeField] private float _airDrag = 0.1f;          // Ma sát trên trời (Thấp để bay xa)
    [SerializeField] private float _gravityForce = 20f;      // Lực hút xuống đất (Để bám đường như F1)
    [SerializeField] private float _gripFactor = 0.95f;      // 1 = Bám chặt, 0.9 = Drift nhẹ, 0.5 = Drift như sàn nước đá
    
    [Header("Cấu hình Phanh & Ổn định")]
    [SerializeField] private float _brakeDrag = 5f;          // Ma sát khi đạp phanh (Càng cao càng đứng khựng lại)
    [SerializeField] private float _stabilitySpeed = 2f;     // Tốc độ tự cân bằng xe (Hồi phục lại vị trí đứng thẳng)
    [SerializeField] private float _maxTiltAngle = 60f;      // Góc nghiêng tối đa cho phép (Quá góc này là xe tự bẻ lại)

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;    // Điểm bắn tia dưới gầm xe
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _rayLength = 1.0f;

    private Rigidbody _rb;
    private float _moveInput;
    private float _steerInput;
    private bool _isBraking;
    private bool _isGrounded;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        // Hạ thấp trọng tâm xe xuống để đỡ bị lật như con lật đật
        _rb.centerOfMass = new Vector3(0, -0.5f, 0); 
    }

    void Update()
    {
        // 1. Chỉ lấy Input ở đây
        _moveInput = Input.GetAxisRaw("Vertical");
        _steerInput = Input.GetAxisRaw("Horizontal");
        _isBraking = Input.GetKey(KeyCode.Space); // Phanh bằng phím Space
    }

    void FixedUpdate()
    {
        CheckGround();
        ApplyGravity();
        
        if (_isGrounded)
        {
            MoveAndBrake(); // Gộp logic di chuyển và phanh
            Steer();
            ApplyDrift();
            Stabilize(); // Giữ xe không bị lật
        }
        else
        {
            // Trên trời thì giảm ma sát để bay
            _rb.drag = _airDrag;
            
            // Cho phép xoay nhẹ trên không để tiếp đất nghệ thuật (Air Control)
            _rb.AddTorque(transform.up * _steerInput * _turnSpeed * 0.2f); 
            
            // Tự cân bằng ngay cả khi đang bay để tiếp đất bằng bánh xe
            Stabilize(); 
        }
    }

    private void CheckGround()
    {
        // Bắn tia từ gầm xe xuống
        _isGrounded = Physics.Raycast(_groundCheckPoint.position, -transform.up, out RaycastHit hit, _rayLength, _groundLayer);
        
        // Vẽ tia debug để ông biết gầm xe đang ở đâu
        Debug.DrawRay(_groundCheckPoint.position, -transform.up * _rayLength, _isGrounded ? Color.green : Color.red);
    }

    private void MoveAndBrake()
    {
        // Xử lý Phanh trước
        if (_isBraking)
        {
            // Khi phanh, tăng Drag lên cực cao để xe dừng lại nhanh chóng
            _rb.drag = _brakeDrag;
            // Không áp dụng lực động cơ khi đang đạp phanh
            return; 
        }
        
        // Nếu không phanh thì trả về Drag bình thường khi chạm đất
        _rb.drag = _groundDrag;

        // Tính tốc độ hiện tại theo hướng tiến của xe
        float currentSpeed = Vector3.Dot(_rb.velocity, transform.forward);

        // Logic tăng tốc:
        // Nếu đang chạy quá nhanh thì đừng đẩy thêm nữa (nhưng vẫn cho phép phanh/lùi)
        bool isSpeeding = currentSpeed > _maxSpeed && _moveInput > 0;
        bool isReversingTooFast = currentSpeed < -_reverseSpeed && _moveInput < 0;

        if (!isSpeeding && !isReversingTooFast)
        {
            // Dùng AddForce để đẩy. ForceMode.Acceleration bỏ qua khối lượng -> Xe nhẹ hay nặng chạy như nhau.
            _rb.AddForce(transform.forward * _moveInput * _acceleration, ForceMode.Acceleration);
        }
    }

    private void Steer()
    {
        // Chỉ bẻ lái được khi xe đang lăn bánh (đứng im xoay vô lăng thì xe không quay)
        // Dùng sqrMagnitude để đỡ tốn chi phí tính căn bậc 2
        if (_rb.velocity.sqrMagnitude > 0.1f)
        {
            // Nếu đi lùi, đảo ngược hướng lái cho đúng thực tế
            float direction = Vector3.Dot(_rb.velocity, transform.forward) >= 0 ? 1f : -1f;
            
            float turn = _steerInput * _turnSpeed * Time.fixedDeltaTime * direction;
            
            // Xoay trực tiếp Rotation của Rigidbody (Ổn định hơn AddTorque cho game Arcade)
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            _rb.MoveRotation(_rb.rotation * turnRotation);
        }
    }

    private void ApplyDrift()
    {
        // Đây là ma thuật Drift
        // Lấy vận tốc ngang (Vận tốc trượt sang hai bên hông xe)
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);
        
        // Giảm bớt vận tốc ngang đi. 
        // GripFactor càng gần 1 thì càng triệt tiêu trượt ngang -> Xe bám đường.
        localVelocity.x *= _gripFactor;

        // Chuyển ngược lại thành World Velocity và gán vào Rigidbody
        _rb.velocity = transform.TransformDirection(localVelocity);
    }

    private void ApplyGravity()
    {
        // Luôn luôn hút xe xuống đất, kể cả khi trên dốc
        // Giúp xe bám dốc tốt hơn gravity mặc định của Unity
        _rb.AddForce(Vector3.down * _gravityForce, ForceMode.Acceleration);
    }

    private void Stabilize()
    {
        // Tính toán hướng "Lên" của xe so với hướng "Lên" của thế giới (Vector3.up)
        // Chúng ta muốn xe luôn hướng đầu lên trời, không lật ngửa
        
        Vector3 predictedUp = Quaternion.AngleAxis(
            _rb.angularVelocity.magnitude * Mathf.Rad2Deg * _stabilitySpeed / _stabilitySpeed,
            _rb.angularVelocity
        ) * transform.up;

        // Tính lực xoắn cần thiết để đưa xe về vị trí cân bằng
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        
        // Chỉ áp dụng lực cân bằng khi xe nghiêng quá mức hoặc đang bay
        // Giúp xe có cảm giác nghiêng tự nhiên khi cua, nhưng không bị lật
        float angle = Vector3.Angle(transform.up, Vector3.up);
        if (angle > _maxTiltAngle || !_isGrounded)
        {
             _rb.AddTorque(torqueVector * _stabilitySpeed * _stabilitySpeed, ForceMode.Acceleration);
        }
    }
}