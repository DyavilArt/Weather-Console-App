using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

using Weather_Console_App.Models;

namespace Weather_Console_App.Service
{
    public class WeatherService
    {
        private readonly HttpClient _http;
        private readonly string _urlGeocode = "https://geocoding-api.open-meteo.com/v1/search";
        private readonly string _urlWeather = "https://api.open-meteo.com/v1/forecast";
        private readonly Dictionary<string, (HourlyData Weather, DateTime CachedAt)> _weatherCache = new();
        private readonly Dictionary<string, (GeocodingResult City, DateTime CachedAt)> _cityCache = new();
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(60);
        public WeatherService()
        {
            _http = new HttpClient();
        }
        public async Task<GeocodingResult?> GetCoordinatesAsync(string cityName)
        {
            if (_cityCache.TryGetValue(cityName, out var cached) &&
                DateTime.Now - cached.CachedAt < _cacheDuration)
            {
                return cached.City;
            }
            try
            {
                string Url = $"{_urlGeocode}?name={cityName}";
                HttpResponseMessage responseMessage = await _http.GetAsync(Url);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Проблема с интернетом, либо API вернул ошибку.");
                    return null;
                }
                string json = await responseMessage.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var data = JsonSerializer.Deserialize<GeocodingResponse>(json, options);
                if (data == null || data.Results == null || data.Results.Count == 0)
                {
                    Console.WriteLine("Город не найден. Проверьте название.");
                    return null;
                }
                var city = data.Results[0];
                _cityCache[cityName] = (city, DateTime.Now);
                return city;
            }
            catch
            {
                Console.WriteLine("Неизвестная ошибка при запросе к API.");
                return null;
            }
        }
        public async Task<HourlyData?> GetWeatherAsync(double latitude, double longitude)
        {
            if (_weatherCache.TryGetValue($"{latitude},{longitude}", out var cached) &&
                DateTime.Now - cached.CachedAt < _cacheDuration)
            {
                return cached.Weather;
            }
            try
            {
                string Url = $"{_urlWeather}?latitude={latitude}&longitude={longitude}&hourly=temperature_2m,weather_code,pressure_msl";
                HttpResponseMessage responseMessage = await _http.GetAsync(Url);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Проблема с интернетом, либо API вернул ошибку.");
                    return null;
                }
                string json = await responseMessage.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var data = JsonSerializer.Deserialize<WeatherResponse>(json, options);
                if (data == null || data.Hourly == null || data.Hourly.Temperature == null
                    || data.Hourly.Pressure == null || data.Hourly.WeatherCode == null)
                {
                    Console.WriteLine("Нет данных о погоде в данном городе.");
                    return null;
                }
                var weather = data.Hourly;
                _weatherCache[$"{latitude},{longitude}"] = (weather, DateTime.Now);
                return weather;

            }
            catch
            {
                Console.WriteLine("Неизвестная ошибка при запросе к API.");
                return null;
            }
        }
        public async Task<(GeocodingResult? City, HourlyData? Weather)> GetWeatherForCityAsync(string cityName)
        {
            var city = await GetCoordinatesAsync(cityName);
            if (city == null)
                return (null, null);
            var weather = await GetWeatherAsync(city.Latitude, city.Longitude);
            return (city, weather);
        }
    }
}
