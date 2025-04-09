package by.tyshkevich.evacuation_system.General.Services;

import org.json.JSONObject;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;


@Service
public class WeatherService
{
    @Value("ab3a442e3e33cb52dda464f495110325")
    private String apiKey;

    @Value("http://api.openweathermap.org/data/2.5/weather")
    private String weatherUrl;

    private final RestTemplate restTemplate = new RestTemplate();

    public JSONObject getWeatherByCoordinates(double lat, double lon)
    {
        String url = String.format("%s?lat=%f&lon=%f&units=metric&appid=%s", weatherUrl, lat, lon, apiKey);
        ResponseEntity<String> response = restTemplate.getForEntity(url, String.class);
        return new JSONObject(response.getBody());
    }

    public JSONObject getWeatherByCity(String city)
    {
        String url = String.format("%s?q=%s&units=metric&appid=%s", weatherUrl, city, apiKey);
        ResponseEntity<String> response = restTemplate.getForEntity(url, String.class);
        return new JSONObject(response.getBody());
    }

    public String determineDangerLevel(JSONObject weatherData)
    {
        JSONObject main = weatherData.getJSONObject("main");
        double temp = main.getDouble("temp");
        int pressure = main.getInt("pressure");

        double windSpeed = weatherData.getJSONObject("wind").getDouble("speed");

        String dangerLevel = "Низкий";
        if (temp > 35 || temp < -10 || windSpeed > 15 || pressure < 1000) {
            dangerLevel = "Высокий";
        } else if (temp > 30 || temp < 0 || windSpeed > 10) {
            dangerLevel = "Умеренный";
        }

        return dangerLevel;
    }
}
