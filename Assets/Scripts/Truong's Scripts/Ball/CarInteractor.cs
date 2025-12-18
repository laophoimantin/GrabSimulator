using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInteractor : MonoBehaviour, IInteractor
{
    [Header("Detector Settings")]
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactRadius = 0.5f;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;

    private bool _canInteract = true;
    private IInteractable _currentInteractable;
    [SerializeField] private GameObject _cinemachineObj;
    
    [SerializeField] private CarControllerLegacy _carController;

    // Update is called once per frame
    void Update()
    {
        if (!_canInteract) return;

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

        foreach (Collider interactableCollider in colliders)
        {
            // Tránh trường hợp Xe tự detect chính nó (nếu xe cũng có Collider thuộc InteractLayer)
            if (interactableCollider.transform.root == transform.root) continue;

            if (interactableCollider.TryGetComponent(out IInteractable interactable))
            {
                float distance = Vector3.Distance(_interactionPoint.position, interactableCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInteractable = interactable;
                }
            }
        }
        
        _currentInteractable = nearestInteractable;
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

    public void SetCanMove(bool state)
    {
        _carController.SetCanMove(state);
        if (state)
        {
            CinemachineManager.Instance.SetNewCamera(_cinemachineObj);
            Debug.Log("Set Camera");
        }
    }
}
