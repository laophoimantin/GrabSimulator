using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpringJoint))]
public class SpringStackTweak : MonoBehaviour
{
    [SerializeField] float _turnAssistRate;
    [SerializeField] float _stayMass;
    [SerializeField] float _ChaseLength, _ChaseSpringMultiplier;
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
        bool isFar = stackDistance >= _ChaseLength;

        if (isFar)
        {
            m_Rigidbody.mass = 1;
            m_SpringJoint.spring = springBase * ((stackDistance - _ChaseLength > 1) ? stackDistance - _ChaseLength : 1) * _ChaseSpringMultiplier;
            m_Rigidbody.drag = 0;
        }
        else
        {
            if (_turnAssistRate < Quaternion.Angle(m_Rigidbody.rotation, rotationBase) && Quaternion.Angle(m_Rigidbody.rotation, rotationBase)  < 180 - _turnAssistRate)
            {
                Debug.Log(this.gameObject.name + " YEARN TO SPIN: " + Quaternion.Angle(m_Rigidbody.rotation, rotationBase));
                Quaternion target = Quaternion.RotateTowards(m_Rigidbody.rotation, rotationBase, _turnAssistRate * Time.fixedDeltaTime);
                m_Rigidbody.MoveRotation(target);
            }
            else
            {
                m_Rigidbody.mass = _stayMass;
                m_Rigidbody.drag = dragBase;
                m_SpringJoint.spring = springBase;
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
