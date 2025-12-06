using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Dectector Settings")]
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactRadius = 0.5f;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;

    private IInteractable _currentInteractable;

    [SerializeField] private DialogueUI _dialogueUI;
    public DialogueUI DialogueUI => _dialogueUI;
    
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

    private void OnDrawGizmosSelected()
    {
        if (_interactionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_interactionPoint.position, _interactRadius);
        }
    }
}
