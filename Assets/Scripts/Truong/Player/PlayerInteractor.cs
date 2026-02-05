using UnityEngine;

public class PlayerInteractor : MonoBehaviour, IInteractor
{
    [Header("Detector Settings")]
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactRadius = 0.5f;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;

    private bool _canInteract = true;
    private IInteractable _currentInteractable;
    
    private readonly Collider[] _colliderBuffer = new Collider[10];
        
    void Update()
    {
        _currentInteractable = null;
        
        if (!_canInteract) return;
        
        DetectInteractable();
        
        if (_currentInteractable != null && Input.GetKeyDown(_interactKey))
        {
            _currentInteractable.Interact(this);
        }
    }

    private void DetectInteractable()
    {
        int hits = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactRadius, _colliderBuffer, _interactLayer);

        if (hits == 0) return; 

        IInteractable nearestInteractable = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < hits; i++)
        {
            if (_colliderBuffer[i].TryGetComponent(out IInteractable interactable))
            {
                float distance = Vector3.Distance(_interactionPoint.position, _colliderBuffer[i].transform.position);
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
        throw new System.NotImplementedException();
    }
}