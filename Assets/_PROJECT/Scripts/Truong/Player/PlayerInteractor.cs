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
    
    private IInteractable _currentInteractable;
    
    private PhysicalCargo _heldCargo;
    [SerializeField] private Transform _handBoneR;
    [SerializeField] private Transform _handBoneL;

    private Vector3 _originLocationR;
    private Quaternion _originRotationR;
    private Vector3 _originLocationL;
    private Quaternion _originRotationL;

    [SerializeField] private Vector3 _holdLocationR;
    [SerializeField] private Vector3 _holdRotationR;
    [SerializeField] private Vector3 _holdLocationL;
    [SerializeField] private Vector3 _holdRotationL;

    private Quaternion _holdRotQuatR;
    private Quaternion _holdRotQuatL;

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

    void Start()
    {
        if (_handBoneR != null)
        {
            _originLocationR = _handBoneR.localPosition;
            _originRotationR = _handBoneR.localRotation;
            _holdRotQuatR = Quaternion.Euler(_holdRotationR); 
        }

        if (_handBoneL != null)
        {
            _originLocationL = _handBoneL.localPosition;
            _originRotationL = _handBoneL.localRotation;
            _holdRotQuatL = Quaternion.Euler(_holdRotationL);
        }
    }
    private void OnInteractInput(InputAction.CallbackContext context)
    {
        if (InputLocker.IsLocked(InputActionType.Interact)) return;

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
        if (InputLocker.IsLocked(InputActionType.Interact))
        {
            _currentInteractable = null;
            return;
        }

        DetectInteractable();
    }

    private void LateUpdate()
    {
        if (_handBoneR == null || _handBoneL == null) return;

        _handBoneL.localPosition = _heldCargo ? _holdLocationR : _originLocationR;
        _handBoneL.localRotation = _heldCargo ? _holdRotQuatR : _originRotationR;

        _handBoneR.localPosition = _heldCargo ? _holdLocationL : _originLocationL;
        _handBoneR.localRotation = _heldCargo ? _holdRotQuatL : _originRotationL;
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