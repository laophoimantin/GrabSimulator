using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildRoad : MonoBehaviour
{
    [SerializeField] MeshFilter m_meshfilter;

    private void Awake()
    {
        buildMesh();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void OnSplineChange()
    {

    }
    private void buildMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        int offset = 0;

        int length = SplineSampler.instance._vertsP2.Count;

        for (int i = 1; i<=length; i++)
        {
            Vector3 p1 = SplineSampler.instance._vertsP1[i - 1];
            Vector3 p2 = SplineSampler.instance._vertsP2[i - 1];
            Vector3 p3;
            Vector3 p4;

            if (i == length)
            {
                p3 = SplineSampler.instance._vertsP1[0];
                p4 = SplineSampler.instance._vertsP2[0];
            }
            else
            {
                p3 = SplineSampler.instance._vertsP1[i];
                p4 = SplineSampler.instance._vertsP2[i];
            }

            offset = 4 * (i - 1);

            int t1 = offset + 0;
            int t2 = offset + 2;    
            int t3 = offset + 3;

            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset + 0;

            verts.AddRange(new List<Vector3> { p1, p2, p3, p4 });
            tris.AddRange(new List<int> { t1, t2, t3, t4, t5, t6 });
        }

        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m_meshfilter.mesh = m;
    }
}