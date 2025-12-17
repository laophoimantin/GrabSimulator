using UnityEngine;

public class PlayerCamController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientation;
    [SerializeField] private Transform _playerObj;
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private GameObject _cinemachineObj;

    [Header("Settings")]
    [SerializeField] private float _rotationSpeed;

    private float _horizontalInput;
    private float _verticalInput;


    void Start()
    {
        CursorManager.Instance.HideCursor();

        if (_mainCamera == null)
            if (Camera.main != null)
                _mainCamera = Camera.main.transform;
        CinemachineManager.Instance.SetNewCamera(_cinemachineObj);
    }

    void Update()
    {
        if (CursorManager.Instance.IsCursorActive)
            return;

        MyInput();

        // Rotate orientation 
        Vector3 viewDir = _playerObj.position - _mainCamera.position;
        viewDir.y = 0;
        _orientation.forward = viewDir.normalized;

        // Rotate the player object
        RotateModel();
    }

    private void RotateModel()
    {
        // Calculate direction relative to camera orientation
        Vector3 inputDir = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;
        if (inputDir != Vector3.zero)
        {
            _playerObj.forward = Vector3.Slerp(_playerObj.forward, inputDir.normalized, _rotationSpeed * Time.deltaTime);
        }
    }

    private void MyInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }

    public void ShowCamera()
    {
        CinemachineManager.Instance.SetNewCamera(_cinemachineObj);
    }


    #region DEBUG>>>

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(_orientation.position, _orientation.forward * 3);
    }
#endif

    #endregion
}


// Graveyard

// [Header("Camera Switching")]
// [SerializeField] private GameObject _combatCamObj;
//[SerializeField] private KeyCode _switchKey = KeyCode.V;
//[SerializeField] private Transform _combatLookAt;

// private enum PlayerCameraStyle
// {
//     Basic,
//     Combat
// }
//[SerializeField] private PlayerCameraStyle _currentStyle = PlayerCameraStyle.Basic;

// private void RotateCombat(Vector3 viewDir)
// {
//     _playerObj.forward = Vector3.Slerp(_playerObj.forward, viewDir, _rotationSpeed * 5 * Time.deltaTime);
// }


// private void SwitchCameraInput()
// {
//     if (Input.GetKeyDown(_switchKey))
//     {
//         if (_currentStyle == PlayerCameraStyle.Basic) SetStyle(PlayerCameraStyle.Combat);
//         else SetStyle(PlayerCameraStyle.Basic);
//     }
// }
//
// private void SetStyle(PlayerCameraStyle style)
// {
//     _currentStyle = style;
//     _basicCamObj.SetActive(_currentStyle == PlayerCameraStyle.Basic);
//     _combatCamObj.SetActive(_currentStyle == PlayerCameraStyle.Combat);
// }


// if (_currentStyle == PlayerCameraStyle.Basic)
// {
// }
// else if (_currentStyle == PlayerCameraStyle.Combat)
// {
//     Vector3 dirToCombatLookAt = _combatLookAt.position - new Vector3(_mainCamera.position.x, _combatLookAt.position.y, _mainCamera.position.z);
//     _orientation.forward = dirToCombatLookAt.normalized;
//     //_playerObj.forward = dirToCombatLookAt.normalized;
//     RotateCombat(dirToCombatLookAt.normalized);
// }