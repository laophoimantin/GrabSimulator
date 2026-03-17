using UnityEngine;

[RequireComponent(typeof(MotorbikePhysics))]
[RequireComponent(typeof(MotorbikeInput))]
public class MotorbikeVisuals : MonoBehaviour
{
    [SerializeField] private GameObject _dummyModel;

    [Header("Wheels")]
    [SerializeField] private Transform _frontWheel;
    [SerializeField] private Transform _rearWheel;
    [SerializeField] private float _wheelRotSpeed = 200f;

    [Header("Handle")]
    [SerializeField] private Transform _handle;
    [SerializeField] private float _handleRotVal = 30f;
    [SerializeField] private float _handleRotSpeed = 0.1f;

    [Header("Skid Marks")]
    [SerializeField] private TrailRenderer _skidMarks;
    [SerializeField] private float _skidWidth = 0.1f;
    [SerializeField] private float _minSkidVelocity = 3f;

    
    private MotorbikePhysics _physics;
    private MotorbikeInput _input;

    private void Awake()
    {
        _physics = GetComponent<MotorbikePhysics>();
        _input = GetComponent<MotorbikeInput>();

        _skidMarks.startWidth = _skidWidth;
        _skidMarks.emitting = false;
    }

    private void LateUpdate()
    {
        RotateWheels();
        RotateHandle();
        UpdateSkidMarks();
    }

    // -------------------------------------------------------------------------
    // Visual methods
    // -------------------------------------------------------------------------
    private void RotateWheels()
    {
        _frontWheel.Rotate(Vector3.forward, -_wheelRotSpeed * Time.deltaTime * _physics.CurrentVelocityOffset);
        _rearWheel.Rotate(Vector3.forward, -_wheelRotSpeed * Time.deltaTime * _input.MoveInput);
    }

    private void RotateHandle()
    {
        Quaternion targetRot = Quaternion.Euler(
            _handle.localRotation.eulerAngles.x,
            _handleRotVal * _input.SteerInput,
            _handle.localRotation.eulerAngles.z
        );

        _handle.localRotation = Quaternion.Slerp(
            _handle.localRotation,
            targetRot,
            _handleRotSpeed
        );
    }

    private void UpdateSkidMarks()
    {
        bool isSideslipping = _physics.IsGrounded && Mathf.Abs(_physics.LateralVelocity) > _minSkidVelocity;

        _skidMarks.emitting = isSideslipping || _input.IsBraking;
    }

    public void SetDummyModelState(bool state)
    {
        _dummyModel.SetActive(state);
    }
}