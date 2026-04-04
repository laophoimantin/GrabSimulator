using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

using UnityEditor;

[ExecuteInEditMode]
public class SplineSampler : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private int _splineIndex;
    [SerializeField, Range(0f, 1f)] private float _timeRange;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float time;

    [SerializeField] private float width = 0.5f;

    float3 _position;
    float3 _forward;
    float3 _upVector;
    float3 p1;
    float3 p2;

    private void Update()
    {
        if (_splineContainer == null) return;
        if (Application.isPlaying)
        {
            time += Time.deltaTime * speed;
            if (time > _timeRange) time = 0f;
        }

        _splineContainer.Evaluate(_splineIndex, time, out _position, out _forward, out _upVector);

        Debug.Log($"{time} Position: {_position}, Forward: {_forward}, Up: {_upVector}");
        float3 right = math.normalize(math.cross(_forward, _upVector));

        p1 = _position + (right * width);
        p2 = _position + (-right * width);

    }
    private void OnDrawGizmos()
    {
        if (_splineContainer == null) return;

        Handles.matrix = transform.localToWorldMatrix;

        Handles.color = Color.green;
        Handles.SphereHandleCap(0, _position, Quaternion.identity, 0.2f, EventType.Repaint);

        Handles.color = Color.red;
        Handles.SphereHandleCap(0, p1, Quaternion.identity, 0.1f, EventType.Repaint);
        Handles.SphereHandleCap(0, p2, Quaternion.identity, 0.1f, EventType.Repaint);

        Handles.color = Color.white;
        Handles.DrawLine(_position, p1);
        Handles.DrawLine(_position, p2);

        Gizmos.DrawSphere(_position, 1);
        Gizmos.DrawSphere(_position, 1);
    }
}
//using Unity.Mathematics;
//using UnityEditor;
//using UnityEngine;
//using UnityEngine.Splines;

//[ExecuteInEditMode]
//public class SplineSampler : MonoBehaviour
//{
//    [SerializeField] private SplineContainer _splineContainer;
//    [SerializeField] private int _splineIndex;
//    [SerializeField, Range(0f, 1f)] private float _time;

//    float width = 0.5f;

//    float3 _position;
//    float3 _foward;
//    float3 _upVector;
//    float3 p1;
//    float3 p2;


//    private void Update()
//    {
//        _splineContainer.Evaluate(_splineIndex, _time, out _position, out _foward, out _upVector);

//        float3 right = Vector3.Cross(_foward, _upVector).normalized;
//        p1 = _position + (right * width);  
//        p2 = _position + (-right * width);  
//        Debug.Log($"{_splineIndex} Position: {_position}, Forward: {_foward}, Up: {_upVector}");

//    }

//    private void OnDrawGizmos()
//    {
//        Handles.matrix = transform.localToWorldMatrix;
//        Handles.SphereHandleCap(0, _position, Quaternion.identity, 1f, EventType.Repaint);
//    }
//}
