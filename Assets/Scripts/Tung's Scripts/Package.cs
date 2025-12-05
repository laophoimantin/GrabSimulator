using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class Package : MonoBehaviour
{
    [SerializeField] float recallDistance, recallSpring, snapDistance;
    [SerializeField] PackageStack stack;

    bool is_in_place; public bool isInPlace => is_in_place;
    Vector3 globalAnchor, globalConnectedAnchor;
    SpringJoint _sj;
    Rigidbody _rb;

    private void Start()
    {
        _sj = GetComponent<SpringJoint>();
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        globalAnchor = transform.TransformPoint(_sj.anchor);
        globalConnectedAnchor = _sj.connectedBody.transform.TransformPoint(_sj.connectedAnchor);
        if (is_in_place)
        {
            _rb.position = globalConnectedAnchor;
            _rb.rotation = _sj.connectedBody.transform.rotation;
            _sj.spring = 0;
            return;
        }

        float stackDistance = (globalAnchor - globalConnectedAnchor).magnitude;
        if(stackDistance < recallDistance)
        {
            _sj.spring = recallSpring;
            if (stackDistance < snapDistance)
            {
                Snap();
            }
        }
    }

    public void SetTarget(Rigidbody newTarget)
    {
        _sj.connectedBody = newTarget;
    }

    public void Snap()
    {
        is_in_place = true;
        _rb.position = globalConnectedAnchor;
        _rb.rotation = _sj.connectedBody.transform.rotation;
        _sj.spring = 0;
        stack.PointMoveOn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(globalAnchor, globalConnectedAnchor);
    }

}
