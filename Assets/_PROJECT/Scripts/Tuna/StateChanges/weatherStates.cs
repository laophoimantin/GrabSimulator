using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherStateChanges
{
    private WeatherState _currentWeatherState;
    public WeatherStateChanges()
    {
        _currentWeatherState = WeatherState.Sunny;
    }
    public void ChangeWeather(WeatherState newState)
    {
        OnWeatherExit(_currentWeatherState);
        _currentWeatherState = newState;
        OnWeatherEnter(newState);
    }

    void OnWeatherExit(WeatherState oldState)
    {
        switch (oldState)
        {
            case WeatherState.Sunny:
                Debug.Log("Exited Sunny: clear skies and bright sun.");
                break;
            case WeatherState.SunnyWithLightRain:
                Debug.Log("Exited SunnyWithLightRain: sun with a light drizzle.");
                break;
            case WeatherState.Cloudy:
                Debug.Log("Exited Cloudy: overcast skies with no rain.");
                break;
            case WeatherState.CloudyWithLightRain:
                Debug.Log("Exited CloudyWithLightRain: overcast with a light drizzle.");
                break;
            case WeatherState.CloudyWithHeavyRain:
                Debug.Log("Exited CloudyWithHeavyRain: overcast with heavy rain.");
                break;
            case WeatherState.Thunderstorm:
                Debug.Log("Exited Thunderstorm: heavy rain with thunder and lightning.");
                break;
            case WeatherState.Foggy:
                Debug.Log("Exited Foggy: low visibility due to fog.");
                break;
            case WeatherState.NiceDay:
                Debug.Log("Exited NiceDay: perfect weather for outdoor activities.");
                break;
        }
    }

    void OnWeatherEnter(WeatherState newState)
    {
        switch (newState)
        {
            case WeatherState.Sunny:
                Debug.Log("Entered Sunny: clear skies and bright sun.");
                break;
            case WeatherState.SunnyWithLightRain:
                Debug.Log("Entered SunnyWithLightRain: sun with a light drizzle.");
                break;
            case WeatherState.Cloudy:
                Debug.Log("Entered Cloudy: overcast skies with no rain.");
                break;
            case WeatherState.CloudyWithLightRain:
                Debug.Log("Entered CloudyWithLightRain: overcast with a light drizzle.");
                break;
            case WeatherState.CloudyWithHeavyRain:
                Debug.Log("Entered CloudyWithHeavyRain: overcast with heavy rain.");
                break;
            case WeatherState.Thunderstorm:
                Debug.Log("Entered Thunderstorm: heavy rain with thunder and lightning.");
                break;
            case WeatherState.Foggy:
                Debug.Log("Entered Foggy: low visibility due to fog.");
                break;
            case WeatherState.NiceDay:
                Debug.Log("Entered NiceDay: perfect weather for outdoor activities.");
                break;
        }
    }
}
