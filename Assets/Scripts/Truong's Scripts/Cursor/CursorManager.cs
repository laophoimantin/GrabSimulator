using Cinemachine;
using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    public bool IsCursorActive => Cursor.visible;

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        LockCinemachine();
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UnlockCinemachine();
    }

    private void LockCinemachine()
    {
        CinemachineCore.GetInputAxis = (axisName) => 0f;
    }

    private void UnlockCinemachine()
    {
        CinemachineCore.GetInputAxis = (axisName) => Input.GetAxis(axisName);
    }
}