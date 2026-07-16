using System.Text.Json.Serialization;

namespace Weather_Console_App.Models
{
    public class GeocodingResponse
    {
        [JsonPropertyName("results")]
        public List<GeocodingResult> Results { get; set; } = new();
    }
}