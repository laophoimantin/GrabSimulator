using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorbikeController : MonoBehaviour
{
    [SerializeField] private float _motorForce = 100f;
    [SerializeField] private float _breakForce = 1000f;
    [SerializeField] private float _maxSteerAngle = 1000f;
    
    [SerializeField] private WheelCollider _frontWheelCollider;
    [SerializeField] private WheelCollider _rearWheelCollider;
    
    [SerializeField] private Transform _frontWheelTransform;
    [SerializeField] private Transform _rearWheelTransform;
    
    private float _horizontalInput;
    private float _verticalInput;
    private float _currentSteerAngle;
    private float _currentBreakForce;
    private bool _isBraking;
    
    

    void Update()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        ApplyBraking();
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _isBraking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        _frontWheelCollider.motorTorque = _motorForce * _verticalInput;
        _rearWheelCollider.motorTorque = _motorForce * _verticalInput;
        
        _currentBreakForce = _isBraking ? _breakForce : 0f;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        _frontWheelCollider.brakeTorque = _currentBreakForce;
        _rearWheelCollider.brakeTorque = _currentBreakForce;
    }

    private void HandleSteering()
    {
        _currentSteerAngle = _maxSteerAngle * _horizontalInput;
        _frontWheelCollider.steerAngle = _currentSteerAngle;
        //_rearWheelCollider.steerAngle = _currentSteerAngle;
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot * Quaternion.Euler(0, 0, 90);;
        //wheelTransform.rotation = rot;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(_frontWheelCollider, _frontWheelTransform);
        UpdateSingleWheel(_rearWheelCollider, _rearWheelTransform);
    }
}
