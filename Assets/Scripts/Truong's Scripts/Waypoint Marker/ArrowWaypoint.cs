using UnityEngine;

public class ArrowWaypoint : MonoBehaviour
{
    [SerializeField] private Transform _arrow;
    [SerializeField] private Transform _follow;
    
    [SerializeField] private Transform _target;

    void Update()
    {
        _arrow.position = _follow.position;
        
        Vector3 relativePos = _target.position - _arrow.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        _arrow.rotation = rotation;
    }
}