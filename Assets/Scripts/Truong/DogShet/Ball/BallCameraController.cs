using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCameraController : MonoBehaviour
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

    
    void Start()
    {
        CursorManager.Instance.HideCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(_switchKey))
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
