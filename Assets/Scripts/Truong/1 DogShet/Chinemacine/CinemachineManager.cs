using UnityEngine;

public class CinemachineManager : Singleton<CinemachineManager>
{
    private GameObject _currentCamera;
    
    public void SetNewCamera(GameObject newCamera)
    {
        if (newCamera == null)
            return;
        if (_currentCamera == newCamera) 
            return;
        
        
        newCamera.SetActive(true);
        if (_currentCamera != null)
        {
            _currentCamera.SetActive(false);
        }
        _currentCamera = newCamera;
    }
}