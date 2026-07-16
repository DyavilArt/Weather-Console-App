using System.Text.Json.Serialization;

namespace Weather_Console_App.Models
{
    public class HourlyData
    {
        [JsonPropertyName("temperature_2m")]
        public List<double> Temperature { get; set; } = new();
        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode { get; set; } = new();
        [JsonPropertyName("pressure_msl")]
        public List<double> Pressure { get; set; } = new();
    }
}