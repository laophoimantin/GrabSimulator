using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    
    [Header("Target Config")]
    [SerializeField] private Transform _targetToFollow; // Drag the Physics Ball here
    [SerializeField] private Rigidbody _targetRb; // Drag the Ball's Rigidbody here (needed for velocity)


    [Header("Follow Config")]
    [SerializeField] private Vector3 _offset; 
    [SerializeField] private float _smoothSpeed = 10f; 

    void LateUpdate()
    {
        if (_targetToFollow == null) return;

        // 1. POSITION: Follow the ball smoothly
        Vector3 targetPos = _targetToFollow.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _smoothSpeed);
    }
}