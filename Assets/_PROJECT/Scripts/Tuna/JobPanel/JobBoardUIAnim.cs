using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class JobBoardUIAnim : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private RectTransform _panelRect;

    [Header("Animation Settings")]
    [SerializeField] private float _slideDuration = 0.4f;
    [SerializeField] private Ease _openEase = Ease.OutBack;
    [SerializeField] private Ease _closeEase = Ease.InBack;
    [SerializeField] private float _yTopOffset = 100f;

    private readonly float _hiddenPosY = 0f;
    private float _targetPosY;

    private bool _isOpen = false;
    private InputAction _toggleAction;

    private void Start()
    {
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            _toggleAction = InputManager.Instance.InputActions.UI.TogglePanel;
            _toggleAction.started += OnTogglePressed;
        }

        _targetPosY = _canvasRect.rect.height - _yTopOffset;
        
        _panelRect.anchoredPosition = new Vector2(_panelRect.anchoredPosition.x, _hiddenPosY);
        _panelRect.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_toggleAction != null)
            _toggleAction.started -= OnTogglePressed;
    }

    private void OnTogglePressed(InputAction.CallbackContext ctx)
    {
        ToggleJobBoard();
    }

    private void ToggleJobBoard()
    {
        _isOpen = !_isOpen;
        _panelRect.DOKill();

        if (_isOpen)
        {
            _panelRect.gameObject.SetActive(true);
            
            InputLocker.Lock(InputActionType.Interact, this);
            CursorLocker.RequestCursor(this);
            
            _panelRect.DOAnchorPosY(_targetPosY, _slideDuration)
                .SetEase(_openEase)
                .SetUpdate(true);
        }
        else
        {
            
            InputLocker.Unlock(InputActionType.Interact, this);
            CursorLocker.ReleaseCursor(this);

            _panelRect.DOAnchorPosY(_hiddenPosY, _slideDuration)
                .SetEase(_closeEase)
                .SetUpdate(true)
                .OnComplete(() => { _panelRect.gameObject.SetActive(false); });
        }
    }
}