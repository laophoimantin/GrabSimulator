using Cinemachine;
using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    // bool IsCursorActive => Cursor.visible;
    public bool IsCursorActive => Cursor.lockState != CursorLockMode.Locked;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsCursorActive) HideCursor();
        }
    }
    
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
        CinemachineCore.GetInputAxis = Input.GetAxis;
    }
}