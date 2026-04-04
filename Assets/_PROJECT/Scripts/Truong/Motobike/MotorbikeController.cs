using UnityEngine;

/// <summary>
/// Prototype
/// </summary>
public class MotorbikeController : MonoBehaviour
{
    [SerializeField] private MotorbikeMovement _movement;
    [SerializeField] private MotorbikeVisuals _visuals;

    void Start()
    {
        HideDummyModel();
    }

    public void LockMovement()
    {
        _movement.SetMovementLock(true);
    }

    public void UnlockMovement()
    {
        _movement.SetMovementLock(false);
    }

    public void HideDummyModel()
    {
        _visuals.SetDummyModelState(false);
    }

    public void ShowDummyModel()
    {
        _visuals.SetDummyModelState(true);
    }
}