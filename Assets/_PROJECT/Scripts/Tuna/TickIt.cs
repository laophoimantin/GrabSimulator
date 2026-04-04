using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickIt : MonoBehaviour
{
    [HideInInspector] public static TickIt Instance;
    [HideInInspector] private int tickTime;
    [HideInInspector] private float tickTimer;

    private void Awake()
    {
        tickTime = Mathf.RoundToInt(GameRule.TICK_INTERVAL / Time.fixedDeltaTime);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        tickTimer++;
        if (tickTimer == tickTime)
        {
            Debug.Log("Tick");
            GameRule.TICK = true;
            tickTimer = 0;
        }
        else
        {
            GameRule.TICK = false;
        }
    }
}
