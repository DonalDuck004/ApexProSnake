using System.Text.Json.Serialization;

namespace ApexProSnake.Gamesense
{
    public class BindGameEvent<T> : GamesenseRequestEventObject where T : BindGameEventHandlerData
    {
        [JsonPropertyName("min_value")]
        public int MinValue { get; init; } = 0;

        [JsonPropertyName("max_value")]
        public int MaxValue { get; init; } = 100;

        [JsonPropertyName("icon_id")]
        public int? IconID { get; init; } = null;

        [JsonPropertyName("handlers")]
        public required BindGameEventHandler<T>[] Handlers { get; init; }

        [JsonPropertyName("data_fields")]
        public object[] DataFields { get; init; } = new object[0];
    }
}
