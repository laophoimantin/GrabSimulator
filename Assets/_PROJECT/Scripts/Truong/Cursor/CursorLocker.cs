using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public static class CursorLocker
{
    private static readonly HashSet<object> _requesters = new();
    public static bool IsCursorActive => _requesters.Count > 0;

    public static void RequestCursor(object source)
    {
        _requesters.Add(source);
        UpdateCursorState();
    }

    public static void ReleaseCursor(object source)
    {
        _requesters.Remove(source);
        UpdateCursorState();
    }
    public static void ForceClearAll()
    {
        _requesters.Clear();
        UpdateCursorState();
    }
    public static void Init()
    {
        _requesters.Clear(); 
        UpdateCursorState(); 
    }

    private static float GetNewMouseAxis(string axisName)
    {
        if (InputManager.Instance == null || InputManager.Instance.InputActions == null) 
            return 0f;

        Vector2 mouseDelta = InputManager.Instance.InputActions.MouseInput.MouseLook.ReadValue<Vector2>();

        if (axisName == "Mouse X") return mouseDelta.x;
        if (axisName == "Mouse Y") return mouseDelta.y;

        return 0f;
    }
    
    private static void UpdateCursorState()
    {
        bool needsCursor = _requesters.Count > 0;

        Cursor.lockState = needsCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = needsCursor;

        if (needsCursor)
        {
            CinemachineCore.GetInputAxis = (axisName) => 0f;
        }
        else
        {
            CinemachineCore.GetInputAxis = GetNewMouseAxis;
        }
    }
}