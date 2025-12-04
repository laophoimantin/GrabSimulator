using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorbikeEntrySystem : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private MotorbikeController _motorbikeControllerScript; 
    [SerializeField] private GameObject _carCamera;
    [SerializeField] private Transform _exitPoint; 
    
    [Header("Settings")]
    [SerializeField] private KeyCode _exitKey = KeyCode.E;

    private GameObject _player; 
    private bool _isDriving = false;

    void Start()
    {
        _motorbikeControllerScript.enabled = false;
        _carCamera.SetActive(false);
    }

    void Update()
    {
        if (_isDriving && Input.GetKeyDown(_exitKey))
        {
            ExitCar();
        }
    }

    public void Interact(PlayerInteractor player)
    {
        _player = player.gameObject;
        if (_player != null)
            EnterCar();
    }

    public string GetInteractionPrompt()
    {
        return "Drive";
    }

    private void EnterCar()
    {
        _isDriving = true;
        _player.SetActive(false);
        _player.transform.SetParent(transform);
        _motorbikeControllerScript.enabled = true;

        _carCamera.SetActive(true);
    }

    private void ExitCar()
    {
        _isDriving = false;

        _player.transform.SetParent(null); 
        _player.transform.position = _exitPoint.position;
        _player.transform.rotation = _exitPoint.rotation;

        _player.SetActive(true);

        _motorbikeControllerScript.enabled = false;

        _carCamera.SetActive(false);
    }
}
