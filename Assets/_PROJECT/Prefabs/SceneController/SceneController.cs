using System;
using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private bool _isLoading;

    [Header("Settings")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private bool _dontDestroyOnLoad = true;

    [Header("Loading Screen")]
    [SerializeField] private Image _blockerPanel;
    [SerializeField] private RectTransform _circle;
    [SerializeField] private float _duration = 1f;
    private Vector2 _originalSize;

    [Header("Whole canvas")]
    [SerializeField] private GameObject _canvas;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


    }

    void Start()
    {
        if (_blockerPanel != null) _blockerPanel.raycastTarget = false;
        _originalSize = _circle.sizeDelta;

        _canvas.SetActive(false);
    }
    #region Public API

    /// Standard Level Transition: Fades out, loads a new scene, fades in.
    public void LoadNewScene(string sceneName)
    {
        LoadScene(sceneName);
    }

    public void LoadGameplayScene()
    {
        LoadScene(SceneName.Gameplay);
    }

    public void LoadMainMenu()
    {
        LoadScene(SceneName.MainMenu);
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }


    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    #endregion


    #region Internal Logic

    private void LoadScene(string sceneName)
    {
        if (_isLoading) return;
        _isLoading = true;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }


    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        _canvas.SetActive(true);
        if (_blockerPanel != null) _blockerPanel.raycastTarget = true;

        // PHASE 1: TRANSITION TO LOADING SCREEN
        // =============================================================================

        yield return _circle.DOSizeDelta(Vector2.zero, _duration)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true)
            .WaitForCompletion();

        InputLocker.ForceClearAll();
        // PHASE 2: LOADING
        // =============================================================================

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false; // Prevent auto-jumping

        // While loading...
        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //if (_progressBar != null)
            //_progressBar.value = progress;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        // PHASE 3: TRANSITION TO NEW SCENE
        // =============================================================================
        operation.allowSceneActivation = true;
        while (!operation.isDone)
            yield return null;

        yield return _circle.DOSizeDelta(_originalSize, _duration)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true)
            .WaitForCompletion();

        if (_blockerPanel != null) _blockerPanel.raycastTarget = false;
        _isLoading = false;
        _canvas.SetActive(false);
    }

    #endregion
}

public static class SceneName
{
    public const string MainMenu = "MainMenu";
    public const string Gameplay = "Map Blockout";
}


public static class ScreenFader
{
    public static void OnFadeIn(CanvasGroup group, float duration, Action callback = null)
    {
        group.DOFade(1f, duration).OnComplete(() => callback?.Invoke());
    }

    public static void OnFadeOut(CanvasGroup group, float duration, Action callback = null)
    {
        group.DOFade(0f, duration).OnComplete(() => callback?.Invoke());
    }

    public static Tween FadeIn(CanvasGroup group, float duration)
        => group.DOFade(1f, duration);

    public static Tween FadeOut(CanvasGroup group, float duration)
        => group.DOFade(0f, duration);
}