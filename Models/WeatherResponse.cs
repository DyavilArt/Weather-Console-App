using System.Text.Json.Serialization;

namespace Weather_Console_App.Models
{
    public class WeatherResponse
    {
        [JsonPropertyName("hourly")]
        public HourlyData Hourly { get; set; } = new();
    }
}