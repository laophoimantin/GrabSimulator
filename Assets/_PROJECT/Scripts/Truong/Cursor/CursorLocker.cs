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
        InputManager.Instance.InputActions.MouseInput.Enable();
        if (axisName == "Mouse X") return mouseDelta.x;
        if (axisName == "Mouse Y") return mouseDelta.y;

        return 0f;
    }
    public static bool IsUsingFakeCursor = false;
    private static void UpdateCursorState()
    {
        bool needsCursor = _requesters.Count > 0;

        if (needsCursor)
        {
            if (IsUsingFakeCursor)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            CinemachineCore.GetInputAxis = (axisName) => 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CinemachineCore.GetInputAxis = GetNewMouseAxis; 
        }
    }

    public static void RefreshCursor()
    {
        UpdateCursorState();
    }
}