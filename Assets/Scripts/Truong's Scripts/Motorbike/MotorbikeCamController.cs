using UnityEngine;

public class MotorbikeCamController : MonoBehaviour
{
    private enum CameraStyle
    {
        FirstPerson,
        ThirdPerson
    }

    [Header("Camera Switching")]
    [SerializeField] private GameObject _firstPersonCamObj;
    [SerializeField] private GameObject _thirdPersonCamObj;
    [SerializeField] private KeyCode _switchKey = KeyCode.V;
    [SerializeField] private CameraStyle _currentStyle = CameraStyle.ThirdPerson;

    public bool IsDriving { get; set; } = false;
    
    void Start()
    {
        _firstPersonCamObj.SetActive(false);
        _thirdPersonCamObj.SetActive(false);
    }

    void Update()
    {
        if (IsDriving && Input.GetKeyDown(_switchKey))
        {
            ToggleCameraStyle();
        }
    }

    public void ShowCamera()
    {
        ActivateCamera(_currentStyle);
    }

    private void ToggleCameraStyle()
    {
        if (_currentStyle == CameraStyle.ThirdPerson)
            _currentStyle = CameraStyle.FirstPerson;
        else
            _currentStyle = CameraStyle.ThirdPerson;

        ActivateCamera(_currentStyle);
    }

    private void ActivateCamera(CameraStyle style)
    {
        GameObject targetCam = (style == CameraStyle.ThirdPerson) ? _thirdPersonCamObj : _firstPersonCamObj;
        CinemachineManager.Instance.SetNewCamera(targetCam);
    }
}

// Graveyard
// [Header("Target Settings")]
    // [SerializeField] private Transform _target;
    // [SerializeField] private Transform _firstPersonAnchor; // optional: bike handlebar or rider head
    // [SerializeField] private Vector3 _thirdPersonOffset = new Vector3(0, 2f, -5f);
    //
    // [Header("Camera Settings")]
    // [SerializeField] private float _followSpeed = 5f;
    // [SerializeField] private float _rotationSpeed = 5f;
    // [SerializeField] private float _lookAheadFactor = 2f;
    // [SerializeField] private float _leanTiltAmount = 10f;
    //
    // [Header("View Mode")]
    // [SerializeField] private bool _isFirstPerson = false;
    // [SerializeField] private KeyCode _switchKey = KeyCode.V;
    //
    // private Vector3 _currentVelocity;
    //
    // void LateUpdate()
    // {
    //     if (!_target) return;
    //
    //     if (Input.GetKeyDown(_switchKey))
    //         _isFirstPerson = !_isFirstPerson;
    //
    //     if (_isFirstPerson && _firstPersonAnchor != null)
    //     {
    //         transform.position = _firstPersonAnchor.position;
    //         transform.rotation = _firstPersonAnchor.rotation;
    //     }
    //     else
    //     {
    //         Vector3 desiredPosition = _target.TransformPoint(_thirdPersonOffset);
    //         transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, 1f / _followSpeed);
    //
    //         Vector3 lookPoint = _target.position + _target.forward * _lookAheadFactor;
    //         Quaternion targetRot = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
    //
    //         float leanAngle = GetLeanAngle(_target);
    //         Quaternion tiltRot = Quaternion.AngleAxis(leanAngle * _leanTiltAmount, _target.forward);
    //
    //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRot * tiltRot, _rotationSpeed * Time.deltaTime);
    //     }
    // }
    //
    // private float GetLeanAngle(Transform bike)
    // {
    //     Vector3 localUp = bike.up;
    //     return Vector3.SignedAngle(Vector3.up, localUp, bike.forward) / 45f; 
    // }





