using UnityEngine;

[RequireComponent(typeof(MotorbikeInput))]
public class MotorbikePhysics : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rbSphere;
    [SerializeField] private Rigidbody _rbBikeBody;

    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 30f;
    [SerializeField] private float _acceleration = 5f;

    [Header("Steering")]
    [SerializeField] private float _steerStrength = 15f;
    [SerializeField] private AnimationCurve _turningCurve;

    [Header("Tilt")]
    [SerializeField] private float _zTiltAngle = 10f;
    [SerializeField] private float _bikeXTiltIncrement = 0.1f;

    [Header("Braking")]
    [Range(0, 10)]
    [SerializeField] private float _brakingFactor = 5f;
    [SerializeField] private float _driftDrag = 5f;
    [SerializeField] private float _normalDrag = 1f;

    [Header("Ground")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _gravity = 20f;


    public float LateralVelocity { get; private set; }
    public float CurrentVelocityOffset { get; private set; }
    public bool IsGrounded { get; private set; }
    public RaycastHit GroundHit { get; private set; }

    private MotorbikeInput _input;
    private float _rayLength;
    private RaycastHit _hit;

    private void Awake()
    {
        _input = GetComponent<MotorbikeInput>();
        _rayLength = _rbSphere.GetComponent<SphereCollider>().radius + 0.2f;

        //_rbSphere.transform.parent = null;
        //_rbBikeBody.transform.parent = null;
    }

    void Update()
    {
        transform.position = _rbSphere.transform.position;
    }

    private void FixedUpdate()
    {
        // 1. Tính xem xe đang trôi dạt (drift) hay đang đi thẳng
        UpdateVelocityOffset();

        IsGrounded = CheckGrounded();
        GroundHit = _hit;

        if (IsGrounded)
        {
            if (!_input.IsBraking)
                Accelerate();

            Rotate();
            Brake();
        }
        else
        {
            ApplyGravity();
        }

        TiltBike();
    }

    /// <summary>
    /// Đọc velocity của _rbBikeBody thay vì _rbSphere
    /// vì _rbBikeBody đã oriented theo hướng xe, InverseTransformDirection
    /// mới cho ra local Z = tiến/lùi đúng nghĩa
    /// </summary>
    private void UpdateVelocityOffset()
    {
        // InverseTransformDirection:Chuyển đổi từ World Space sang Local Space
        // Cục Sphere đang lăn với vận tốc 50km/h về hướng Bắc (World Space). Nhưng xe đang quay mặt về hướng Đông
        // Hàm này sẽ trả lời câu hỏi: "So với CÁI XE, thì nó đang trượt ngang bao nhiêu, tiến tới bao nhiêu?"
        Vector3 localVel = _rbBikeBody.transform.InverseTransformDirection(_rbBikeBody.velocity);

        //Chia cho _maxSpeed để normalize: 1.0 = full speed tiến, -1.0 = full speed lùi
        // Không clamp để Rotate() vẫn hoạt động khi lùi (giá trị âm đảo chiều lái)
        // Nói chung là xem xe đang chạy bao nhiêu phần trăm so với max speed, dm
        //CurrentVelocityOffset = Mathf.Clamp01(localVel.z / _maxSpeed);
        CurrentVelocityOffset = localVel.z / _maxSpeed;
        // Trục X: Trái/Phải. Nếu số này to tức là xe đang bị văng đít về đâu (Drift). Nói chung là để xong xe có đang drift không
        //Trục X local: dương = văng sang phải, âm = văng sang trái
        // Giá trị tuyệt đối càng lớn thì drift càng mạnh
        // BikeVisuals đọc cái này để quyết định có bật skid marks không
        LateralVelocity = localVel.x;
    }

    // private void Accelerate()
    // {
    //     // _rbSphere.velocity = Vector3.Lerp(
    //     //     _rbSphere.velocity,
    //     //     _input.MoveInput * _maxSpeed * transform.forward,
    //     //     _acceleration * Time.fixedDeltaTime
    //     // );
    //     Vector3 vel = _rbSphere.velocity; // cái bóng đang đi về hướng nào với tốc độ bao nhiêu
    //     Vector3 target = _input.MoveInput * _maxSpeed * transform.forward; // mình đang muốn xe đi về đâu, (tến lên hay lùi) * (phía trước của xe) * (max tốc độ)
    //     Vector3 horizontal = Vector3.Lerp(new Vector3(vel.x, 0, vel.z), target, _acceleration * Time.fixedDeltaTime); // hướng, lực để đẩy cái xe, không tính y
    //     
    //     _rbSphere.velocity = new Vector3(horizontal.x, vel.y, horizontal.z); // áp vào, không thay đổi lực y
    // }

    private void Accelerate()
    {
        //Nếu chưa có raycast hit nào ( RaycastHit mặc định, normal = zero) thì fallback về Vector3.up — coi như mặt đất phẳng
        Vector3 groundNormal = GroundHit.normal != Vector3.zero ? GroundHit.normal : Vector3.up;

        // ÉP HƯỚNG ĐI TRƯỢT THEO MẶT DỐC
        // Mũi xe trỏ đi đâu, ép nó nằm song song với mặt đất dưới lốp xe bấy nhiêu. 
        // Nếu không dùng, leo dốc sẽ khó khăn hơn
        // Nói chung là bẻ cái forward của thằng transform song song với mặt phẳng, hỏi chat đi dm
        // Đm giả dụ cái trục forward nó cắm vào dốc, mà dốc thì nghiêng thì nếu nghiêng thì cùng một độ dài nhưng mà lúc nghiêng thì nó 
        // có x với y khác nhau, và ở phần tính velo cuối cùng thì nó bỏ y đi vì tránh ovveride trọng lực, thế nên x lúc leo dốc phải ít hơn 
        // khi đang chạy trên mặt phẳng bình thường, thì làm leo dốc nó "thực hơn', chậm hơn cho giống đời thực, chắc thế đcm 
        Vector3 moveDirection = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;

        // Vận tốc mục tiêu: Hướng chuẩn x Ga x Tốc độ tối đa.
        Vector3 targetVelocity = moveDirection * (_input.MoveInput * _maxSpeed);

        // Vận tốc hiện tại của cục Sphere.
        Vector3 currentVelocity = _rbSphere.velocity;

        // Lerp chỉ trên mặt phẳng XZ — bỏ Y của targetVelocity
        // vì trên dốc, sphere tự leo theo collider, không cần ép velocity Y
        // Nếu ép Y theo moveDirection, trọng lực sẽ bị override
        Vector3 horizontalVelocity = Vector3.Lerp(
            new Vector3(currentVelocity.x, 0, currentVelocity.z),
            new Vector3(targetVelocity.x, 0, targetVelocity.z),
            _acceleration * Time.fixedDeltaTime
        );

        // không ảnh hưởng đến y hiện tại
        _rbSphere.velocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
    }

    private void Rotate()
    {
        // _input.SteerInput: Trái/phải ( -1 đến 1 )
        // _input.MoveInput: Tiến/lùi ( -1 đến 1 ). Nhân cái này vào để khi lùi xe, trái phải bị đảo ngược cho đúng thực tế.
        // _turningCurve.Evaluate(Mathf.Abs(CurrentVelocityOffset)): ĐỒ THỊ BẺ LÁI.
        // Giúp cấm không cho bẻ lái khi xe đang đứng im (vận tốc = 0), hoặc giảm góc cua khi chạy quá nhanh (vận tốc = 1) để tránh lật xe.
        // CurrentVelocityOffset: -1 (Lùi hết ga)  1 (Tiến hết ga) 0.5 (Nửa ga )
        // Mathf.Abs(CurrentVelocityOffset): Việc bẻ lái không quan tâm đến số âm, chỉ cần biết đang bẻ bao nhiêu thôi đm
        // _turningCurve.Evaluate: cắm CurrentVelocityOffset vào time
        // không di chuyển (offset = 0) không bẻ được lái
        // di chuyển một it (offset = 0.3 hay gì đó) bẻ một ít theo trục y của biểu đồ
        // di chuyển vai l (max speed) ôm cua hết cỡ
        float turnAmount = _input.SteerInput * _input.MoveInput // đảo chiều bẻ lái lại khi lùi, tự làm toán đi lmao
                                             * _turningCurve.Evaluate(Mathf.Abs(CurrentVelocityOffset))
                                             * _steerStrength * Time.fixedDeltaTime; // bẻ lực bao nhiêu

        // Xoay theo trục Y 
        transform.Rotate(0f, turnAmount, 0f, Space.World);
    }

    private void Brake()
    {
        if (_input.IsBraking)
        {
            // Bóp phanh kiểu hủy diệt. Mỗi 0.02s (FixedUpdate), vận tốc bị nhân với (5/10) = 0.5.
            // Có nghĩa là tốc độ giảm theo hàm mũ.
            _rbSphere.velocity *= _brakingFactor / 10f;
            // Tăng sức cản không khí/ma sát lên cao
            _rbSphere.drag = _driftDrag;
        }
        else
        {
            // Nhả phanh ra thì trả ma sát về bình thường
            _rbSphere.drag = _normalDrag;
        }
    }

    private bool CheckGrounded()
    {
        // Quét một cục cầu từ trên xuống dưới (SphereCast).
        float radius = _rayLength - 0.02f;
        Vector3 origin = _rbSphere.position + radius * Vector3.up;

        return Physics.SphereCast(origin, radius + 0.02f, -transform.up, out _hit, _rayLength, _groundLayer);
        //return Physics.SphereCast(origin, radius + 0.02f, Vector3.down, out _hit, _rayLength, _groundLayer);
    }

    private void ApplyGravity()
    {
        // cái gravity của thằng Unity bị lỏ, phải tự custom dcm
        _rbSphere.AddForce(_gravity * Vector3.down, ForceMode.Acceleration);
    }

    private void TiltBike()
    {
        // 1. NGHIÊNG THEO ĐỘ DỐC (Pitch - Trục X)
        // FromToRotation tính ra góc cần xoay để trục Up (trục hướng lên đỉnh đầu) của xe 
        // khớp với Normal (trục vuông góc với mặt đường - _hit.normal). Lên dốc thì ngóc đầu, xuống dốc thì cắm mỏ.
        float xRot = (Quaternion.FromToRotation(_rbBikeBody.transform.up, _hit.normal) * _rbBikeBody.transform.rotation).eulerAngles.x;

        // 2. NGHIÊNG KHI VÀO CUA (Roll - Trục Z)
        // Lái sang trái (_input) * Tốc độ hiện tại. Chạy càng nhanh bẻ lái thì úp cua càng gắt (như MotoGP).
        // Tự đọc đi
        float zRot = CurrentVelocityOffset > 0f
            ? -_zTiltAngle * _input.SteerInput * Mathf.Clamp01(CurrentVelocityOffset)
            : 0f;

        // Nội suy từ từ (Slerp) cái dáng xe từ dáng hiện tại sang cái dáng mong muốn (Target)
        // Chú ý: Trục Y (yaw) nó lấy trực tiếp từ transform cha (do hàm Rotate() ở trên đã bẻ cái cha rồi).
        // Tự đọc đi
        Quaternion targetRot = Quaternion.Slerp(
            _rbBikeBody.transform.rotation,
            Quaternion.Euler(xRot, transform.eulerAngles.y, zRot),
            _bikeXTiltIncrement
        );

        // Cuối cùng là áp nó vào cái Body của xe.
        _rbBikeBody.MoveRotation(Quaternion.Euler(targetRot.eulerAngles.x, transform.eulerAngles.y, targetRot.eulerAngles.z));
    }
}