using System.Text.Json.Serialization;

namespace ApexProSnake.Gamesense
{
    public class GameEvent<T> : GamesenseRequestEventObject where T : GameEventData
    {
        [JsonPropertyName("data")]
        public required T Data { get; init; }
    }
}
