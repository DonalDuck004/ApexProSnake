using System.Text.Json.Serialization;

namespace ApexProSnake.Gamesense
{
    public class GameMetadata : GamesenseRequestObject {

        [JsonPropertyName("game_display_name")]
        public required string GameDisplayName { get; init; }
       
        [JsonPropertyName("developer")]
        public required string Developer { get; init; }
    } 
}
