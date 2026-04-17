using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    private void OnEnable()
    {
        _playButton.onClick.AddListener(PlayDaGame);
        _quitButton.onClick.AddListener(QuitDaGame);
    }

    private void PlayDaGame()
    {
        SceneController.Instance.LoadGameplayScene();
    }
    private void QuitDaGame()
    {
        SceneController.Instance.QuitGame();
    }
}
