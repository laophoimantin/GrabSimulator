

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "newWeatherInfo", menuName = "Weather/Weather Info SOTest")]
public class WeatherInfoSO2 : ScriptableObject
{
    public WeatherState weatherState;
    public int reapeatWeight;
    public int reapeatRate;
    public int Duration;
    public ConnectedWeather[] connectedWeathers;
}

[Serializable]
public class ConnectedWeather
{
    public WeatherInfoSO2 weatherInfo;
    public int weight;
}
