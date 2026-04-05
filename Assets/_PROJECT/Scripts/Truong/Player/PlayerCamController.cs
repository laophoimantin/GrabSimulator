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