using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

public class MotorbikeEntrySystem : MonoBehaviour, IInteractable
{
    private PlayerController _driver;
    private VehicleState _state = VehicleState.Empty;

    [Header("References")]
    //[SerializeField] private MotorbikeController _controller;
[SerializeField] private BikeMovement _controller;
    
    [SerializeField] private GameObject _bikeCam;
    [SerializeField] private Transform _exitPoint;

    [Header("Settings")]
    private readonly KeyCode _exitKey = KeyCode.E;

    void Start()
    {
        _bikeCam.SetActive(false);
    }

    void Update()
    {
        if (_state == VehicleState.Occupied && Input.GetKeyDown(_exitKey))
        {
            ExitVehicle();
        }
    }

    public void Interact(IInteractor interactor)
    {
        if (_state != VehicleState.Empty)
            return;

        EnterVehicle(interactor.GetPlayer());
    }

    private void EnterVehicle(PlayerController driver)
    {
        _state = VehicleState.Occupied;
        InputManager.Instance.SetMotorcycleMode();

        _driver = driver;

        _driver.transform.SetParent(_controller.transform);
        _driver.LockInteraction();
        _driver.LockMovement();
        _driver.HideModel();


        _controller.ShowDummyModel();
        _controller.UnlockMovement();
        _bikeCam.SetActive(true);
    }

    private void ExitVehicle()
    {
        if (_state != VehicleState.Occupied)
            return;
        _state = VehicleState.Empty;
        InputManager.Instance.SetPlayerMode();

		_controller.HideDummyModel();
        _controller.LockMovement();


        _driver.ShowModel();
        _driver.UnlockInteraction();
        _driver.UnlockMovement();

        _driver.transform.position = _exitPoint.position;
        _driver.transform.rotation = _exitPoint.rotation;
        _driver.transform.SetParent(null);
        _driver = null;

        _bikeCam.SetActive(false);
    }
}

public enum VehicleState
{
    Empty,
    Occupied
}