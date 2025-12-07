using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerCamController : MonoBehaviour
{
    private enum PlayerCameraStyle
    {
        Basic,
        Combat
    }

    [Header("references")]
    [SerializeField] private Transform _orientation;
    //[SerializeField] private Transform _player;
    [SerializeField] private Transform _playerObj;
    [SerializeField] private Transform _mainCamera;

    [Header("Settings")]
    [SerializeField] private float _rotationSpeed;

    [Header("Camera Switching")]
    [SerializeField] private GameObject _basicCamObj;
    [SerializeField] private GameObject _combatCamObj;
    [SerializeField] private KeyCode _switchKey = KeyCode.V;
    
    [SerializeField] private PlayerCameraStyle _currentStyle = PlayerCameraStyle.Basic;
    
    [SerializeField] private Transform _combatLookAt;


    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        
        if (_mainCamera == null)
            if (Camera.main != null)
                _mainCamera = Camera.main.transform;

        if (_currentStyle == PlayerCameraStyle.Basic) SetStyle(PlayerCameraStyle.Combat);
        else SetStyle(PlayerCameraStyle.Basic);

    }

    void Update()
    {
        SwitchCameraInput();
        
        // Rotate orientation 
        Vector3 viewDir = _playerObj.position - new Vector3(_mainCamera.position.x, _playerObj.position.y, _mainCamera.position.z);
        _orientation.forward = viewDir.normalized;

        // Rotate the player object
        if (_currentStyle == PlayerCameraStyle.Basic)
        {
            RotateBasic();
        }
        else if (_currentStyle == PlayerCameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = _combatLookAt.position - new Vector3(_mainCamera.position.x, _combatLookAt.position.y, _mainCamera.position.z);
            _orientation.forward = dirToCombatLookAt.normalized;
            //_playerObj.forward = dirToCombatLookAt.normalized;
            RotateCombat(dirToCombatLookAt.normalized);
        }
    }

    private void RotateBasic()
    {
        float horizonalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        // Calculate direction relative to camera orientation
        Vector3 inputDir = _orientation.forward * verticalInput + _orientation.right * horizonalInput;

        if (inputDir != Vector3.zero)
        {
            _playerObj.forward = Vector3.Slerp(_playerObj.forward, inputDir.normalized, _rotationSpeed * Time.deltaTime);
        }
    }
    
    private void RotateCombat(Vector3 viewDir)
    {
        _playerObj.forward = Vector3.Slerp(_playerObj.forward, viewDir, _rotationSpeed * 5 * Time.deltaTime);
    }
        
        
        
        

    private void SwitchCameraInput()
    {
        if (Input.GetKeyDown(_switchKey))
        {
            if (_currentStyle == PlayerCameraStyle.Basic) SetStyle(PlayerCameraStyle.Combat);
            else SetStyle(PlayerCameraStyle.Basic);
        }
    }

    private void SetStyle(PlayerCameraStyle style)
    {
        _currentStyle = style;
        _basicCamObj.SetActive(_currentStyle == PlayerCameraStyle.Basic);
        _combatCamObj.SetActive(_currentStyle == PlayerCameraStyle.Combat);
    }
    
}