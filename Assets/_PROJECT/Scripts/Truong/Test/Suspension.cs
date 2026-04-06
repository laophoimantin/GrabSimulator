using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _rayAnchor;
    [SerializeField] private Transform _suspension;

    [Header("Suspension Settings")]
    [SerializeField] private float _targetDistance = 1f;
    [SerializeField] private float _maxDistance = 2f;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _followSpeed = 15f;
    [SerializeField] private bool _instantSnap = false;

    [Header("Suspension Limits (Hành trình phuộc)")]
    [SerializeField] private float _upTravelLimit = 0.2f;
    [SerializeField] private float _downTravelLimit = 0.5f;

    private Vector3 _restingLocalPos;
    private float _minLocalY;
    private float _maxLocalY;

    void Start()
    {
        _restingLocalPos = _suspension.localPosition;

        _minLocalY = _restingLocalPos.y - _downTravelLimit;
        _maxLocalY = _restingLocalPos.y + _upTravelLimit;
    }

    void FixedUpdate()
    {
        Vector3 targetLocalPos = _suspension.localPosition;

        if (Physics.Raycast(_rayAnchor.position, Vector3.down, out var hit, _maxDistance, _groundMask))
        {
            // diff là Cạnh góc vuông kề
            float currentSuspensionHeight = _suspension.position.y - hit.point.y;
            float diff = currentSuspensionHeight - _targetDistance;
            if (Mathf.Abs(diff) > 0.0001f)
            {
                // cos 
                float upY = _suspension.up.y;
                if (Mathf.Abs(upY) > 0.0001f)
                {
                    // moveAmount là Cạnh huyền 
                    float moveAmount = -diff / upY;
                        targetLocalPos = _suspension.localPosition + new Vector3(0, moveAmount, 0);
                }
            }
        }
        else
        {
            targetLocalPos = _restingLocalPos + new Vector3(0, -_downTravelLimit, 0);
        }

        targetLocalPos.y = Mathf.Clamp(targetLocalPos.y, _minLocalY, _maxLocalY);
        
        if (_instantSnap)
        {
            _suspension.localPosition = targetLocalPos;
        }
        else
        {
            float t = 1f - Mathf.Exp(-_followSpeed * Time.fixedDeltaTime);
            _suspension.localPosition = Vector3.Lerp(_suspension.localPosition, targetLocalPos, t);
        }
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_suspension.position, _suspension.position + Vector3.down * _targetDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_rayAnchor.position, _rayAnchor.position + Vector3.down * _maxDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_suspension.position, _suspension.position + _suspension.up * 1f);
    }
#endif
}