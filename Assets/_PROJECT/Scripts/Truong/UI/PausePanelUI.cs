using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PausePanelUI : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _quitButton;

    [SerializeField] private GameObject _pausePanel;
    private InputAction _toggleAction;

   
    private void Start()
    {
        _pausePanel.SetActive(false);

        _returnButton.onClick.AddListener(ReturnDaGame);
        _quitButton.onClick.AddListener(BackToMainMenu);

        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            _toggleAction = InputManager.Instance.InputActions.UI.TogglePausePanel;
            _toggleAction.started += TogglePanel;
        }
    }
    private void OnDestroy()
    {
        if (_toggleAction != null)
        {
            _toggleAction.started -= TogglePanel;
        }
    }

    private void TogglePanel(InputAction.CallbackContext ctx)
    {
        if (_pausePanel.activeSelf)
        {
            ReturnDaGame();
        }
        else
        {
            OpenPanel();
        }
    }

    private void OpenPanel()
    {
        CursorLocker.RequestCursor(this);
        InputManager.Instance.StopAllInput();
        _pausePanel.SetActive(true);
    }

    private void ReturnDaGame()
    {
        CursorLocker.ReleaseCursor(this);

        InputManager.Instance.RestorePreviousGameplayState();
        _pausePanel.SetActive(false);
    }
    private void BackToMainMenu()
    {
        SceneController.Instance.LoadMainMenu();
    }
}
