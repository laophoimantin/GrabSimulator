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
	public bool IsHonking { get; private set; }

	public bool CanControl { get; private set; } = true;

    private void Start()
    {
		InputManager.Instance.InputActions.OnBike.Brake.started += OnBrakeStarted;
		InputManager.Instance.InputActions.OnBike.Brake.canceled += OnBrakeCanceled;

		InputManager.Instance.InputActions.OnBike.Honk.started += OnHonkStarted;
		InputManager.Instance.InputActions.OnBike.Honk.canceled += OnHonkCancelled;
	}

    private void OnDisable()
    {
	    if (InputManager.Instance == null) 
		    return;
		InputManager.Instance.InputActions.OnBike.Brake.started -= OnBrakeStarted;
		InputManager.Instance.InputActions.OnBike.Brake.canceled -= OnBrakeCanceled;

        InputManager.Instance.InputActions.OnBike.Honk.started -= OnHonkStarted;
        InputManager.Instance.InputActions.OnBike.Honk.canceled -= OnHonkCancelled;
    }

	private void OnBrakeStarted(InputAction.CallbackContext ctx) => IsBraking = true;
	private void OnBrakeCanceled(InputAction.CallbackContext ctx) => IsBraking = false;

	private void OnHonkStarted(InputAction.CallbackContext ctx) => IsHonking = true;
	private void OnHonkCancelled(InputAction.CallbackContext ctx) => IsHonking = false;

	void Update()
	{
		if (!CanControl)
		{
			MoveInput = 0f;
			SteerInput = 0f;
			IsBraking = false;
			return;
		}

		ReadAxes();
	}

	private void ReadAxes()
	{
		_inputDir = InputManager.Instance.InputActions.OnBike.Move.ReadValue<Vector2>();
		MoveInput = _inputDir.y;
		SteerInput = _inputDir.x;
	}
	
	public void LockMovement() => SetMovementLock( true);
	public void UnlockMovement() => SetMovementLock(false);
	
	private void SetMovementLock(bool isLocked)
	{
		CanControl = !isLocked;

		if (isLocked)
		{
			IsBraking = false;
		}
	}
}
