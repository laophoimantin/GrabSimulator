using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorbikeGroundCheck : MonoBehaviour
{
    [SerializeField] private Rigidbody _rbSphere;
    [SerializeField] private LayerMask _groundLayer;

    public bool IsGrounded { get; private set; }
    public RaycastHit HitInfo { get; private set; }
    public Vector3 HitNormal => IsGrounded ? HitInfo.normal : Vector3.up;

    private float _rayLength;

    void Start()
    {
        _rayLength = _rbSphere.GetComponent<SphereCollider>().radius;
    }

    private void FixedUpdate()
    {
        IsGrounded = CheckGround();
        print(IsGrounded);
    }

    private bool CheckGround()
    {
        float radius = _rayLength;
        Vector3 origin = _rbSphere.position + radius * Vector3.up;
        RaycastHit hit;

        // Quét một cục cầu từ trên xuống dưới (SphereCast).
        if (Physics.SphereCast(origin, radius + 0.02f, -transform.up, out hit, _rayLength, _groundLayer))
        {
            HitInfo = hit;
            return true;
        }

        return false;
    }
}
