using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MotorbikeInput : MonoBehaviour
{
	private Vector2 _inputDir;
	public float MoveInput { get; private set; }
	public float SteerInput { get; private set; }
	public bool IsBraking { get; private set; }

	private bool _isEnabled = true;

    private void OnEnable()
    {
		InputManager.Instance.InputActions.OnBike.Brake.started += OnBrakeStarted;
		InputManager.Instance.InputActions.OnBike.Brake.started += OnBrakeCanceled;

	}

    private void OnDisable()
    {
		InputManager.Instance.InputActions.OnBike.Brake.started -= OnBrakeStarted;
		InputManager.Instance.InputActions.OnBike.Brake.canceled -= OnBrakeCanceled;
	}

	private void OnBrakeStarted(InputAction.CallbackContext ctx) => IsBraking = true;
	private void OnBrakeCanceled(InputAction.CallbackContext ctx) => IsBraking = false;

	void Update()
	{
		if (!_isEnabled)
		{
			MoveInput = 0f;
			SteerInput = 0f;
			return;
		}

		_inputDir = InputManager.Instance.InputActions.OnBike.Move.ReadValue<Vector2>();
		MoveInput = _inputDir.y;
		SteerInput = _inputDir.x;
	}

	public void SetEnabled(bool enabled)
	{
		_isEnabled = enabled;

		if (!enabled)
		{
			IsBraking = false;
		}
	}
}
