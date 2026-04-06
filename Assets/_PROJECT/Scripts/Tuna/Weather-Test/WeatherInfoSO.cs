using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newWeatherInfo", menuName = "Weather/Weather Info SO")]

public class WeatherInfoSO : ScriptableObject
{
    public WeatherState weatherState;
    public WeatherState[] connectedWeathers;
    public int[] weatherWeights;
    public int repeats;
    public int Duration;
}


