using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _defaultParent;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerInteractor _interactor;
    [SerializeField] private PlayerVisualController _visualController;
    [SerializeField] private Transform _handPos;

    [Space(10)]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Collider _collider;
    
    public Transform HandPos => _handPos;
 
    public void MountVehicle(Transform seat)
    {
        SetPhysicsActive(false); 
    
        _visualController.SetModelState(false);

        transform.SetParent(seat);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity; 
    }

    public void DismountVehicle(Transform exitPoint)
    {
        transform.SetParent(_defaultParent);
        transform.position = exitPoint.position;
        transform.rotation = exitPoint.rotation;

        _visualController.SetModelState(true);
    
        SetPhysicsActive(true);
    }
    
    private void SetPhysicsActive(bool isActive)
    {
        _rb.isKinematic = !isActive; 
        _collider.enabled = isActive; 
        _rb.interpolation = isActive ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        _movement.enabled = isActive;
    }

}