using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class SpringStackTweak : MonoBehaviour
{
    [SerializeField] float turnRate;
    [SerializeField] float _catchUpDistance, _followingSpringMultiplier;
    Quaternion rotationBase;
    float springBase;
    float dragBase;

    SpringJoint m_SpringJoint;
    Vector3 globalAnchor, globalConnectedAnchor;
    Rigidbody m_Rigidbody;

    private void Start()
    {
        m_SpringJoint = GetComponent<SpringJoint>();
        m_Rigidbody = GetComponent<Rigidbody>();

        rotationBase = transform.rotation;
        springBase = m_SpringJoint.spring;
        dragBase = m_Rigidbody.drag;
    }

    private void FixedUpdate()
    {
        globalAnchor = transform.TransformPoint(m_SpringJoint.anchor);
        globalConnectedAnchor = m_SpringJoint.connectedBody.transform.TransformPoint(m_SpringJoint.connectedAnchor);
        float stackDistance = (globalAnchor - globalConnectedAnchor).magnitude;
        bool isFar = stackDistance >= _catchUpDistance;

        if (isFar)
        {
            m_SpringJoint.spring = springBase * ((stackDistance - _catchUpDistance > 1) ? stackDistance - _catchUpDistance : 1) * _followingSpringMultiplier;
            m_Rigidbody.drag = 0;
        }
        else
        {
            m_SpringJoint.spring = springBase;
            m_Rigidbody.drag = dragBase;
            
            if(turnRate < Quaternion.Angle(m_Rigidbody.rotation, rotationBase) || Quaternion.Angle(m_Rigidbody.rotation, rotationBase)  < 180- turnRate)
            {
                Debug.Log("I YEARN TO SPIN: " + Quaternion.Angle(m_Rigidbody.rotation, rotationBase));
                Quaternion target = Quaternion.RotateTowards(m_Rigidbody.rotation, rotationBase, turnRate * Time.fixedDeltaTime);
                m_Rigidbody.MoveRotation(target);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawSphere(globalConnectedAnchor, 0.25f);
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(globalAnchor, 0.25f);
    }
}
