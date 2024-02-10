using System.Text.Json.Serialization;

namespace ApexProSnake.Gamesense
{
    public class GamesenseObject { 
    };

    public class GamesenseRequestObject : GamesenseObject
    {
        [JsonPropertyName("game")]
        public required string Game { get; init; }
    }


    public class GamesenseRequestEventObject : GamesenseRequestObject
    {
        [JsonPropertyName("event")]
        public required string Event { get; init; }
    }
}
