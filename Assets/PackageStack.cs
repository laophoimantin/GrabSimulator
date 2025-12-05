using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Animations;

public class PackageStack : MonoBehaviour
{
    [SerializeField] Package[] packages;

    private void FixedUpdate()
    {
        foreach(Package package in packages)
        {
            if(package.is_in_place)
            {
                package.Snap();
                continue;
            }
            else
            {
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.75f);

    }
}
