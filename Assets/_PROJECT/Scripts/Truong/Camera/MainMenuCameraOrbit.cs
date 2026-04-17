using UnityEngine;

public class MainMenuCameraOrbit : MonoBehaviour
{
    [Header("Target Setup")]
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform _target;

    [Header("Orbit Settings")]
    [SerializeField] private float _spinSpeed = 15f; 
    [SerializeField] private float _heightOffset = 5f; 
    [SerializeField] private float _distance = 20f; 

    private void Start()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 startOffset = new Vector3(0, _heightOffset, -_distance);
        _camera.position = _target.position + startOffset;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        _camera.RotateAround(_target.position, Vector3.up, _spinSpeed * Time.deltaTime);

        _camera.LookAt(_target);
    }
}