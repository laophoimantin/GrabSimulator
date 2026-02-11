using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("Motor Config")]
    [SerializeField] private float _acceleration = 10f; 
    [SerializeField] private float _maxSpeed = 15f;     
    
    [Header("Brake & Drift Config")]
    [SerializeField] private float _normalDrag = 0.5f;
    [SerializeField] private float _driftDrag = 2f; // Đã giảm từ 5 xuống 2 để xe trượt đi (Drift) thay vì đứng khựng lại
    [SerializeField] private float _driftControl = 0.5f; // Khả năng điều khiển khi đang drift (0 = mất lái, 1 = như bình thường)
    [SerializeField] private float _airDrag = 0.1f; // Drag khi ở trên không (thường là rất thấp)
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundMask; // Đừng quên set cái này thành "Default" hoặc "Ground"
    [SerializeField] private float _groundRayLength = 0.6f; // Độ dài tia bắn xuống đất (Bán kính bóng + 0.1)

    [Header("References")]
    [SerializeField] private Transform _cameraTransform;

    private Rigidbody _rb;
    private float _horizontalInput;
    private float _verticalInput;
    private bool _isBraking;
    private bool _isGrounded;
    private Vector3 _currentInputDir; // Lưu lại để vẽ Gizmos

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.maxAngularVelocity = 50f; 
        
        if (_cameraTransform == null && Camera.main != null)
            _cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _isBraking = Input.GetKey(KeyCode.Space);
    }

    void FixedUpdate()
    {
        CheckGround();
        ControlDrag();
        MoveBall();
        LimitSpeed();
    }

    private void CheckGround()
    {
        // Bắn một tia từ tâm quả bóng xuống dưới
        // Nếu chạm bất cứ thứ gì thuộc Layer Ground -> Grounded
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _groundRayLength, _groundMask);
    }

    private void MoveBall()
    {
        // Tính toán hướng đi (dùng cho cả việc đẩy bóng và vẽ Gizmos)
        if (_cameraTransform != null)
        {
            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight = _cameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            _currentInputDir = (camForward * _verticalInput + camRight * _horizontalInput).normalized;
        }

        // Logic mới (Drift): Vẫn cho phép điều khiển, nhưng lực yếu đi và Drag thay đổi.
        
        float currentForce = _acceleration;

        if (_isGrounded && _isBraking)
        {
            // Khi Drift: Giảm lực điều khiển đi một chút để cảm giác xe đang trượt ("mất lái" nhẹ)
            currentForce *= _driftControl;
        }

        // Đẩy bóng (Nếu không bấm nút thì _currentInputDir = 0, xe sẽ tự trôi theo quán tính)
        _rb.AddForce(_currentInputDir * currentForce, ForceMode.Acceleration);
    }

    private void ControlDrag()
    {
        if (_isGrounded)
        {
            if (_isBraking)
                _rb.drag = _driftDrag; // Tăng ma sát một chút để "cào đường" (Drift), nhưng không quá cao để dừng hẳn
            else
                _rb.drag = _normalDrag; // Trôi tự do
        }
        else
        {
            // Nếu trên trời: Rất ít ma sát (như rơi tự do)
            _rb.drag = _airDrag;
        }
    }

    private void LimitSpeed()
    {
        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        if (flatVel.magnitude > _maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * _maxSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }
    }

    private void OnDrawGizmos()
    {
        // Vẽ tia Ground Check (Màu xanh lá nếu chạm đất, Đỏ nếu không)
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _groundRayLength);

        // Vẽ hướng di chuyển mong muốn (Màu vàng)
        if (_currentInputDir.sqrMagnitude > 0.1f)
        {
            Gizmos.color = Color.yellow;
            // Vẽ một tia dài 2 mét về hướng đang bấm nút
            Gizmos.DrawRay(transform.position, _currentInputDir * 2f);
            
            // Vẽ cục tròn ở đầu tia cho dễ nhìn
            Gizmos.DrawWireSphere(transform.position + _currentInputDir * 2f, 0.2f);
        }
    }
}