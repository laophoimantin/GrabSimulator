using UnityEngine;

public class MotorbikeController : MonoBehaviour
{
    [SerializeField] private MotorbikeMovement _movement;
    [SerializeField] private MotorbikeVisualController _visualController;

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
        _visualController.SetDummyModelState(false);
    }

    public void ShowDummyModel()
    {
        _visualController.SetDummyModelState(true);
    }
}