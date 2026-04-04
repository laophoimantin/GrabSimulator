using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private TimeStateChange _TimeStateChanges = new();

    private timeState _currentTimeState;

    private int timeElapsed;
    [SerializeField] private int timeStateDuration = 900;

    private void Awake()
    {
        _currentTimeState = timeState.Dawn;

        _TimeStateChanges.ChangeTime(_currentTimeState);
    }

    private void FixedUpdate()
    {
        if (!GameRule.TICK) { return; }
        if (timeElapsed < timeStateDuration)
        {
            timeElapsed++;
        }
        else
        {
            timeElapsed = 0;
            _currentTimeState++;
            _TimeStateChanges.ChangeTime(_currentTimeState);

            Debug.Log($"Time State Changed to {_currentTimeState}");
        }
    }
}
