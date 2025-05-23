using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _weatherUrl;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenWeatherMap:ApiKey"];
        _weatherUrl = configuration["OpenWeatherMap:WeatherUrl"];
        _logger = logger;
    }

    public async Task<JObject> GetWeatherByCoordinates(double lat, double lon)
    {
        var url = $"{_weatherUrl}?lat={lat}&lon={lon}&units=metric&appid={_apiKey}";
        _logger.LogInformation($"WeatherService: Requesting weather data for coordinates: Lat={lat}, Lon={lon}");
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"WeatherService: Failed to get weather data for coordinates: Lat={lat}, Lon={lon}");
            throw;
        }
    }

    public async Task<JObject> GetWeatherByCity(string city)
    {
        var url = $"{_weatherUrl}?q={city}&units=metric&appid={_apiKey}";
        _logger.LogInformation($"WeatherService: Requesting weather data for city: {city}");
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"WeatherService: Failed to get weather data for city: {city}");
            throw;
        }
    }

    public string DetermineDangerLevel(JObject weatherData)
    {
        try
        {
            var main = weatherData["main"];
            var temp = (double)main["temp"];
            var pressure = (int)main["pressure"];
            var windSpeed = (double)weatherData["wind"]["speed"];

            string dangerLevel = "Низкий";
            if (temp > 35 || temp < -10 || windSpeed > 15 || pressure < 1000)
            {
                dangerLevel = "Высокий";
            }
            else if (temp > 30 || temp < 0 || windSpeed > 10)
            {
                dangerLevel = "Умеренный";
            }

            _logger.LogInformation($"WeatherService: Determined danger level: {dangerLevel}, Temp={temp}, WindSpeed={windSpeed}, Pressure={pressure}");
            return dangerLevel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WeatherService: Error determining danger level");
            return "Неизвестно";
        }
    }
}
