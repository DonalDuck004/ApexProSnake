using System.Text.Json.Serialization;

namespace ApexProSnake.Gamesense
{
    public class GameEventDisplayData : GameEventData
    {
        [JsonPropertyName("value")]
        public required int Value { get; init; }

        [JsonPropertyName("frame")]
        public required Dictionary<string, IEnumerable<byte>> Frame { get; init; }
    }
}
