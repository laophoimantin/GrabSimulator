using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    private WeatherInfoSO2 _currentWeather;
    [SerializeField] private WeatherInfoSO2[] allWeatherInfos;
    private int _currentWeatherRepeatWeight;

    private WeatherStateChanges _weatherChanges = new();


    private int weatherTimeElapsed;
    private int weatherRepeated;

    private void Awake()
    {
        SwitchWeather(allWeatherInfos[Random.Range(0, allWeatherInfos.Length)]);
    }


    private void FixedUpdate()
    {
        if (!GameRule.TICK) { return; }

        weatherTimeElapsed++;

        if (weatherTimeElapsed == _currentWeather.Duration)
        {
            weatherTimeElapsed = 0;

            DetermineNextWeather();
        }
    }

    private void DetermineNextWeather()
    {
        int totalWeight = 0;

        bool canRepeat = weatherRepeated < _currentWeather.reapeatRate;
        if (canRepeat)
        {
            totalWeight += _currentWeatherRepeatWeight;
        }

        foreach (var cw in _currentWeather.connectedWeathers)
        {
            totalWeight += cw.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        if (canRepeat)
        {
            currentSum += _currentWeatherRepeatWeight;
            if (randomValue < currentSum)
            {
                RepeatCurrentWeather();
                return;
            }
        }

        foreach (ConnectedWeather connectedWeather in _currentWeather.connectedWeathers)
        {
            currentSum += connectedWeather.weight;
            if (randomValue < currentSum)
            {
                SwitchWeather(connectedWeather.weatherInfo);
                break;
            }
        }
    }

    private void SwitchWeather(WeatherInfoSO2 newWeatherInfo)
    {
        _currentWeather = newWeatherInfo;
        weatherRepeated = 0;
        _currentWeatherRepeatWeight = _currentWeather.reapeatWeight;
        _weatherChanges.ChangeWeather(_currentWeather.weatherState);
        Debug.Log($"Switched to {_currentWeather.weatherState}");
    }

    private void RepeatCurrentWeather()
    {
        weatherRepeated++;
        _currentWeatherRepeatWeight = Mathf.Max(0, _currentWeatherRepeatWeight -= 1);
        Debug.Log($"Repeated {_currentWeather.weatherState} for the {weatherRepeated} time. Current repeat weight: {_currentWeatherRepeatWeight}");
    }

}
