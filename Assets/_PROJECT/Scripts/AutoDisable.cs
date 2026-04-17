using System.Collections;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    [SerializeField] private float _disableDelay = 10f;

    private Coroutine _disableCoroutine;

    private void OnEnable()
    {
        _disableCoroutine = StartCoroutine(DisableAfterDelay());
    }
    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(_disableDelay);
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if (_disableCoroutine != null)
        {
            StopCoroutine(_disableCoroutine);
            _disableCoroutine = null;
        }
    }
}