using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private PlayerMovement _playerMovement;
    public PlayerMovement PlayerMovement => _playerMovement;

    [Header("Detector Settings")]
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactRadius = 0.5f;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;

    private bool _canInteract = true;
    private IInteractable _currentInteractable;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        DetectInteractable();

        if (_currentInteractable != null && Input.GetKeyDown(_interactKey))
        {
            _currentInteractable.Interact(this);
        }
    }

    private void DetectInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(_interactionPoint.position, _interactRadius, _interactLayer);

        IInteractable nearestInteractable = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                float distance = Vector3.Distance(_interactionPoint.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInteractable = interactable;
                }
            }

            _currentInteractable = nearestInteractable;
        }
    }

    public void SetCanInteract(bool canInteract)
    {
        _canInteract = canInteract;
    }

    private void OnDrawGizmosSelected()
    {
        if (_interactionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_interactionPoint.position, _interactRadius);
        }
    }
}