using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
	private MainInputMaps _inputActions;
	public MainInputMaps InputActions => _inputActions;


	protected override void Awake()
	{
		base.Awake();	
		_inputActions = new MainInputMaps();
	}

	private void Start()
	{
		SetPlayerMode();
	}

	private void OnDisable()
	{
		_inputActions.Disable();
	}

	public void SetPlayerMode()
	{
		_inputActions.OnBike.Disable();
		_inputActions.OnGround.Enable();
	}

	public void SetMotorcycleMode()
	{
		_inputActions.OnGround.Disable();
		_inputActions.OnBike.Enable();
	}
}