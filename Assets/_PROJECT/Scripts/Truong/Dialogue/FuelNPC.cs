using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FuelNPC : MonoBehaviour, IInteractable
{
    [SerializeField] private FuelStation _fuelStation;

    [Header("Dialogues")]
    [SerializeField] private DialogueDataSO[] _idleDialogues;
    [SerializeField] private DialogueDataSO[] _refuelFailedDialogues;
    [SerializeField] private DialogueDataSO[] _refuelFullDialogues;
    [SerializeField] private DialogueDataSO[] _refuelSuccessDialogues;
    [SerializeField] private DialogueDataSO[] _refuelNoBikeDialogues;

    [SerializeField] private DialogueActivator _myMouth;
    private PlayerController _buyer;

    public void Interact(IInteractor interactor)
    {
        _buyer = interactor.GetPlayer();
        _myMouth.SpeakRandom(_idleDialogues);
    }

    public void OnRefuelButtonPressed()
    {
        Debug.Log("Refueling");
        RefuelResult result = _fuelStation.TryRefuel();

        switch (result)
        {
            case RefuelResult.Success:
                _myMouth.SpeakRandom(_refuelSuccessDialogues);
                break;
            case RefuelResult.TankAlreadyFull:
                _myMouth.SpeakRandom(_refuelFullDialogues);
                break;
            case RefuelResult.NotEnoughMoney:
                _myMouth.SpeakRandom(_refuelFailedDialogues);
                break;
            case RefuelResult.NoBikeInZone:
                _myMouth.SpeakRandom(_refuelNoBikeDialogues);
                break;
        }
    }
}