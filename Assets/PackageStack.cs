using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Animations;

public class PackageStack : MonoBehaviour
{  
    [SerializeField] Package[] packages;
    [SerializeField] StackPoints[] points;
    int currentPoint;

    //optimization timer
    float timer = 0; float updateTimeCount = 1;

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > updateTimeCount) { timer = 0; }
        else { return; }

        foreach (var package in packages)
        {
            if (package.isInPlace) { continue; }
            package.SetTarget(points[currentPoint].component_rigidbody);
        }
    }

    public void PointMoveOn()
    {
        points[currentPoint].SetOccupied(true);
        currentPoint++;
        foreach (var package in packages)
        {
            if (package.isInPlace) { continue; }
            package.SetTarget(points[currentPoint].component_rigidbody);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.75f);

    }
}
