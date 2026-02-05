using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

public class MotorbikeEntrySystem : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private MotorbikeMovement _bikeMovement;
    [SerializeField] private MotorbikeCamController _bikeCam;
    [SerializeField] private GameObject _fakeRiderVisuals;
    [SerializeField] private Transform _exitPoint;

    [Header("Settings")]
    private readonly KeyCode _exitKey = KeyCode.E;
    
    private PlayerInteractor _playerInteractor;
    private bool _isDriving = false;

    void Start()
    {
        _bikeMovement.enabled = false;
        _fakeRiderVisuals.SetActive(false);
    }

    void Update()
    {
        if (_isDriving && Input.GetKeyDown(_exitKey))
        {
            Dismount();
        }
    }

    public void Interact(PlayerInteractor player)
    {
        _playerInteractor = player;
        Mount();
    }

    private void Mount()
    {
        _isDriving = true;

        //_playerInteractor.PlayerMovement.SetCanMove(false);
        _playerInteractor.gameObject.SetActive(false);
        ToggleBikeState(true);
        
        QuestArrow.Instance.SetAnchor(transform);
    }

    private void Dismount()
    {
        _isDriving = false;

        ToggleBikeState(false);

        // Teleport player back to exit point
        _playerInteractor.gameObject.transform.position = _exitPoint.position;
        _playerInteractor.gameObject.transform.rotation = _exitPoint.rotation;
        
        _playerInteractor.gameObject.SetActive(true);
        //_playerInteractor.PlayerMovement.SetCanMove(true);
        //_playerInteractor.PlayerCamController.ShowCamera();
        
        QuestArrow.Instance.SetAnchor(_playerInteractor.gameObject.transform);
    }

    private void ToggleBikeState(bool status)
    {
        if (!status)
        {
            _bikeMovement.FullStop();
        }

        _bikeMovement.enabled = status;
        _fakeRiderVisuals.SetActive(status);

        if (status)
        {
            _bikeCam.ShowCamera();
        }

        _bikeCam.IsDriving = status;
    }

    public void Interact(IInteractor interactor)
    {
        throw new System.NotImplementedException();
    }
}