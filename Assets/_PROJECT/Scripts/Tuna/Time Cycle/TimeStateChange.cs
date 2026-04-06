using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStateChange
{
    private timeState _currentTimeState;
    public TimeStateChange()
    {
        _currentTimeState = timeState.Midnight;
    }

    public void ChangeTime(timeState newState)
    {
        OnTimeExit(_currentTimeState);
        _currentTimeState = newState;
        OnTimeEnter(newState);
    }

    void OnTimeExit(timeState oldState)
    {
        switch (oldState)
        {
            case timeState.Midnight:
                Debug.Log("Exited Midnight: the world is quiet and dark.");
                break;
            case timeState.Dawn:
                Debug.Log("Exited Dawn: the sky is painted with soft colors as the sun begins to rise.");
                break;
            case timeState.Morning:
                Debug.Log("Exited Morning: the day is fresh and full of potential.");
                break;
            case timeState.Noon:
                Debug.Log("Exited Noon: the sun is at its highest point, casting strong shadows.");
                break;
            case timeState.Afternoon:
                Debug.Log("Exited Afternoon: the day is winding down, with a warm glow in the sky.");
                break;
            case timeState.Dusk:
                Debug.Log("Exited Dusk: the sky is ablaze with colors as the sun sets.");
                break;
            case timeState.Evening:
                Debug.Log("Exited Evening: the world is bathed in a soft, fading light.");
                break;
            case timeState.Night:
                Debug.Log("Exited Night: the stars are out and the world is asleep.");
                break;
        }
    }

    void OnTimeEnter(timeState newState)
    {
        switch (newState)
        {
            case timeState.Midnight:
                Debug.Log("Entered Midnight: the world is quiet and dark.");
                break;
            case timeState.Dawn:
                Debug.Log("Entered Dawn: the sky is painted with soft colors as the sun begins to rise.");
                break;
            case timeState.Morning:
                Debug.Log("Entered Morning: the day is fresh and full of potential.");
                break;
            case timeState.Noon:
                Debug.Log("Entered Noon: the sun is at its highest point, casting strong shadows.");
                break;
            case timeState.Afternoon:
                Debug.Log("Entered Afternoon: the day is winding down, with a warm glow in the sky.");
                break;
            case timeState.Dusk:
                Debug.Log("Entered Dusk: the sky is ablaze with colors as the sun sets.");
                break;
            case timeState.Evening:
                Debug.Log("Entered Evening: the world is bathed in a soft, fading light.");
                break;
            case timeState.Night:
                Debug.Log("Entered Night: the stars are out and the world is asleep.");
                break;
        }
    }
}