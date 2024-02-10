using System.Text.Json.Serialization;

namespace ApexProSnake.Gamesense
{
    public class BindGameEventHandlerDataDisplay : BindGameEventHandlerData
    {
        [JsonPropertyName("has-text")]
        public required bool HasText { get; init; }

        [JsonPropertyName("image-data")]
        public required IEnumerable<byte> ImageData { get; init; }
    }
}
