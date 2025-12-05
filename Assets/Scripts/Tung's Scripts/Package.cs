using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class Package : MonoBehaviour
{
    [SerializeField] float recallDistance, recallTimer;
    [SerializeField] float recallSpring; [SerializeField] Collider recallCollider;

    [HideInInspector] bool isInPlace;
    public bool is_in_place => isInPlace;

    Vector3 globalAnchor, globalConnectedAnchor;
    float timer;

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
        float stackDistance = (globalAnchor - globalConnectedAnchor).magnitude;

        if (stackDistance <= recallDistance && timer < recallTimer)
        {
            isInPlace = true;
        }
        else
        {
            timer += Time.fixedDeltaTime; timer = Mathf.Clamp(timer, 0, recallTimer);
            isInPlace = false;
            _sj.spring = timer/recallTimer * recallSpring;
        }
    }

    public void Snap()
    {
        timer = 0;
        _rb.position = globalConnectedAnchor;
        _rb.rotation = _sj.connectedBody.transform.rotation;
        _sj.spring = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(globalAnchor, globalConnectedAnchor);
    }

}
