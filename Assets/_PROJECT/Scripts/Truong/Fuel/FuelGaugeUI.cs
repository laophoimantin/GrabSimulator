using UnityEngine;

public class FuelGaugeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _needleTransform;
    [SerializeField] private RectTransform _fuelPanel;
    
    [Header("Settings")]
    [SerializeField] private float _fullAngle = 18f;
    [SerializeField] private float _emptyAngle = 85f;
    [SerializeField] private float _smoothSpeed = 5f;

    private FuelSystem _currentFuelSystem;
    private void OnEnable()
    {
        GameEvents.OnPlayerVehicleChanged += UpdateTargetVehicle;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerVehicleChanged -= UpdateTargetVehicle;
    }

    void Start()
    {
        _fuelPanel.gameObject.SetActive(false);
    }
    
    private void UpdateTargetVehicle(FuelSystem newSystem)
    {
        if (newSystem != null)
        {
            _fuelPanel.gameObject.SetActive(true);
            _currentFuelSystem = newSystem;
        }
        else
        {
            _fuelPanel.gameObject.SetActive(false);
            _currentFuelSystem = null;
        }
    }
    private void LateUpdate()
    {
        if (_currentFuelSystem == null || _needleTransform == null) return;

        float percent = _currentFuelSystem.FuelPercent;

        float targetAngle = Mathf.Lerp(_emptyAngle, _fullAngle, percent);

        float currentAngle = _needleTransform.localEulerAngles.z;

        if (currentAngle > 180) currentAngle -= 360;

        float smoothedAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * _smoothSpeed);
        _needleTransform.localEulerAngles = new Vector3(0, 0, smoothedAngle);
    }
}