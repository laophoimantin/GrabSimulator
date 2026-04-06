using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SplineSampler : MonoBehaviour
{
    public static SplineSampler instance;
    
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private int _splineIndex;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float time;
    private float _timeRange = 1;

    [SerializeField] private float width = 0.5f;

    float3 _position;
    float3 _forward;
    float3 _upVector;
    float3 p1;
    float3 p2;

    public List<Vector3> _vertsP1;
    public List<Vector3> _vertsP2;

    [SerializeField] int resolution;

    private void Awake()
    {
        instance = this;
    }
    
    private void Update()
    {
        GetVerts();
    }
   
    private void GetVerts()
    {
        _vertsP1 = new List<Vector3>();
        _vertsP2 = new List<Vector3>();

        float step = 1f / (float)resolution;

        for (int i = 0; i < resolution; i++)
        {
            float t = step * i;
            SampleSplineWidth(t, out Vector3 p1, out Vector3 p2);

            _vertsP1.Add(p1);
            _vertsP2.Add(p2);
        }
    }

    public bool SampleSplineWidth(float t, out Vector3 p1, out Vector3 p2)
    {
        if (_splineContainer == null)
        {
            p1 = Vector3.zero;
            p2 = Vector3.zero;
            return false;
        }
        _splineContainer.Evaluate(_splineIndex, t, out _position, out _forward, out _upVector);
        float3 right = math.normalize(math.cross(_forward, _upVector));
        p1 = _position + (right * width);
        p2 = _position + (-right * width);
        return true;
    }

    private void OnDrawGizmos()
    {
        if (_splineContainer == null) return;
        if (_vertsP1 == null || _vertsP2 == null) return;

        Handles.matrix = transform.localToWorldMatrix;

        //    Gizmos.DrawSphere(p1, (float) 0.1);
        //    Gizmos.DrawSphere(p2, (float) 0.1);

        foreach (var vert in _vertsP1)
        {
            Gizmos.DrawSphere(vert, (float)0.1);
        }
        foreach (var vert in _vertsP2)
        {
            Gizmos.DrawSphere(vert, (float)0.1);
        }
    }
}
#region debug 
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
#endregion