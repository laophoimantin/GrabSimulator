using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugStackPoint : MonoBehaviour
{
    [SerializeField] HingeJoint _sj;
    Vector3 globalAnchor, globalConnectedAnchor;

    private void OnDrawGizmos()
    {
        globalAnchor = transform.TransformPoint(_sj.anchor);
        globalConnectedAnchor = _sj.connectedBody.transform.TransformPoint(_sj.connectedAnchor);
        
        Gizmos.DrawLine(globalAnchor, globalConnectedAnchor);
        Gizmos.DrawSphere(this.gameObject.transform.position, 0.1f);
    }
}
