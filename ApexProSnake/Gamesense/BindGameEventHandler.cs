using System.Text.Json.Serialization;

namespace ApexProSnake.Gamesense
{
    public class BindGameEventHandler<T> : GamesenseObject where T : BindGameEventHandlerData
    {

        [JsonPropertyName("device-type")]
        public required string DeviceType { get; init; }

        [JsonPropertyName("mode")]
        public required string Mode { get; init; }

        [JsonPropertyName("zone")]
        public required string Zone { get; init; }

        [JsonPropertyName("datas")]
        public required T[] Datas { get; init; }
    }
}
