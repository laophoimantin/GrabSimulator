using UnityEngine;
using UnityEngine.InputSystem; 
public class FakeHandCursor : MonoBehaviour
{
    private RectTransform _rectTransform;

    [Header("Settings")]
    [SerializeField] private Vector2 _offset = Vector2.zero;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        CursorLocker.IsUsingFakeCursor = true;
        CursorLocker.RefreshCursor();
    }

    private void OnDisable()
    {
        CursorLocker.IsUsingFakeCursor = false;
        CursorLocker.RefreshCursor();
    }

    private void Update()
    {
        UpdateHandPosition();
    }

    private void UpdateHandPosition()
    {
        Vector2 mousePos;

        if (Mouse.current != null)
        {
            mousePos = Mouse.current.position.ReadValue();
        }
        else return;

        _rectTransform.position = mousePos + _offset;
    }
}