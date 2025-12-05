using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StackPoints : MonoBehaviour
{
    bool is_occupied; public bool isOccupied => is_occupied;
    Rigidbody _rb; public Rigidbody component_rigidbody => _rb;

    public void SetOccupied(bool new_occupied)
    {
        is_occupied = new_occupied;
    }
}
