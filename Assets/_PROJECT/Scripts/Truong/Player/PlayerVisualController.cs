using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    
    [SerializeField] private GameObject _playerModel;


    public void SetModelState(bool state)
    {
        _playerModel.SetActive(state);
    }
}
