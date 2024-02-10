#define USE_DEBUG_PROXY
#undef USE_DEBUG_PROXY

using System.Net.Http;
using System.Net.Http.Json;


namespace ApexProSnake.Gamesense
{
    public class Gamesense
    {
        private const string SERVER_ADDR = "http://127.0.0.1:60281/";
        private HttpClient client = new();

        public Gamesense() {
#if USE_DEBUG_PROXY
            HttpClient.DefaultProxy = new System.Net.WebProxy
            {
                Address = new Uri("http://localhost:8000"),
            };
#endif
        }

        private void Send<T>(T body, string method) where T : GamesenseRequestObject
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, Gamesense.SERVER_ADDR + method);
            msg.Content = JsonContent.Create(body);

            this.client.Send(msg);
        }

        public void GameMetadata(GameMetadata game_metadata) => this.Send(game_metadata, "game_metadata");

        public void BindGameEvent<T>(BindGameEvent<T> bind_game_event) where T : BindGameEventHandlerData
        {
            this.Send(bind_game_event, "bind_game_event");
        }

        public void GameEvent<T>(GameEvent<T> game_event) where T : GameEventData
        {
            this.Send(game_event, "game_event");
        }
    }
}
