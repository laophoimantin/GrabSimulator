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
		SetPlayerInputState();
		_inputActions.UI.Enable();
		_inputActions.MouseInput.Enable();
	}

	private void OnDisable()
	{
		_inputActions.Disable();
	}

	public void SetPlayerInputState()
	{
		_inputActions.OnBike.Disable();
		_inputActions.OnGround.Enable();
	}

	public void DisablePlayerInputState()
	{
		_inputActions.OnGround.Disable();
	}

	public void SetMotorcycleInputState()
	{
		_inputActions.OnGround.Disable();
		_inputActions.OnBike.Enable();
	}
	
}