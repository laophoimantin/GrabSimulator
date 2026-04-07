using System;

public class InputManager : Singleton<InputManager>
{
	private MainInputMaps _inputActions;
	public MainInputMaps InputActions => _inputActions;
	private Action _previousGameplayState;

	protected override void Awake()
	{
		base.Awake();	
		_inputActions = new MainInputMaps();
	}

	private void Start()
	{
		_inputActions.UI.Enable();
		_inputActions.MouseInput.Enable();
		
		SetPlayerInputState();
	}

	private void OnDisable()
	{
		_inputActions.Disable();
	}

	public void SetPlayerInputState()
	{
		_inputActions.OnBike.Disable();
		_inputActions.Dialogue.Disable();
		
		_inputActions.OnGround.Enable();
		
		_previousGameplayState = SetPlayerInputState;
	}


	public void SetMotorcycleInputState()
	{
		_inputActions.OnGround.Disable();
		_inputActions.Dialogue.Disable();
		
		_inputActions.OnBike.Enable();
		
		_previousGameplayState = SetMotorcycleInputState;
	}

	public void SetDialogueStateMode()
	{
		_inputActions.OnGround.Disable();
		_inputActions.OnBike.Disable();
		
		_inputActions.Dialogue.Enable();
	}
	
	public void RestorePreviousGameplayState()
	{
		_previousGameplayState?.Invoke();
	}
}