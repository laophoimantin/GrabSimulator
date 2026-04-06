using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour, IInteractor
{
    [SerializeField] private PlayerController _player;
    public PlayerController GetPlayer() => _player;

    [Header("Detector Settings")]
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactRadius = 0.5f;
    [SerializeField] private LayerMask _interactLayer;

    private bool _canInteract = true;
    private IInteractable _currentInteractable;
    
    private PhysicalCargo _heldCargo;

    private readonly Collider[] _colliders = new Collider[10];

    private void OnEnable()
    {
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            InputManager.Instance.InputActions.OnGround.Interact.performed += OnInteractInput;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            InputManager.Instance.InputActions.OnGround.Interact.performed -= OnInteractInput;
        }
    }

    private void OnInteractInput(InputAction.CallbackContext context)
    {
        if (!_canInteract) return;

        if (_heldCargo != null)
        {
            DropHeldCargo();
            return; 
        }

        if (_currentInteractable != null)
        {
            _currentInteractable.Interact(this);
        }
    }
    
    void Update()
    {
        if (!_canInteract)
        {
            _currentInteractable = null;
            return;
        }

        DetectInteractable();
    }
    
    public void HoldCargo(PhysicalCargo cargo)
    {
        _heldCargo = cargo;
    }

    private void DropHeldCargo()
    {
        _heldCargo.DropFromHands();
        _heldCargo = null;     
    }


    private void DetectInteractable()
    {
        int hits = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactRadius, _colliders, _interactLayer);

        if (hits == 0)
        {
            _currentInteractable = null;
            return;
        }

        IInteractable nearestInteractable = null;
        float nearestDistanceSqr = float.MaxValue;

        for (int i = 0; i < hits; i++)
        {
            if (_colliders[i].TryGetComponent(out IInteractable interactable))
            {
                Vector3 directionToTarget = _colliders[i].transform.position - _interactionPoint.position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < nearestDistanceSqr)
                {
                    nearestDistanceSqr = dSqrToTarget;
                    nearestInteractable = interactable;
                }
            }
        }

        _currentInteractable = nearestInteractable;
    }

    // Lock 
    public void LockInteraction() => _canInteract = false;
    public void UnlockInteraction() => _canInteract = true;


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_interactionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_interactionPoint.position, _interactRadius);
        }
    }
#endif
}