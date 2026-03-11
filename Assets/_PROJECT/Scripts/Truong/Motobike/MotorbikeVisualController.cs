using UnityEngine;

public class MotorbikeVisualController : MonoBehaviour
{
    [SerializeField] private GameObject _dummyModel;
    
    public void SetDummyModelState(bool state)
    {
        _dummyModel.SetActive(state);
    }
}
