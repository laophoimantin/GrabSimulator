using UnityEngine;

public class QuestArrow : Singleton<QuestArrow>
{
    [Header("Settings")]
    [SerializeField] private float _heightOffset = 2.5f;
    [SerializeField] private float _rotationSpeed = 10f; 
    
    [Header("State")]
    [SerializeField] private Transform _anchor;
    [SerializeField] private Transform _target;

    private void Start() 
    {
        gameObject.SetActive(false);
    }

    void LateUpdate() 
    {
        if (_anchor == null || _target == null) return;

        // Hover above the anchor
        Vector3 desiredPos = _anchor.position + (Vector3.up * _heightOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * 15f);

        // Look at the target
        Vector3 direction = _target.position - transform.position;
        direction.y = 0;
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
        }
    }

    public void SetAnchor(Transform newAnchor)
    {
        _anchor = newAnchor;
    }

    public void SetObjective(Transform newTarget)
    {
        _target = newTarget;
        gameObject.SetActive(_target != null);
    }
}