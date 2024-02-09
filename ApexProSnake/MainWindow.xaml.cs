using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;


namespace ApexProSnake
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random random = new();
        private SnakeBlock food;
        private HttpClient client = new();
        private int cycle = 1;

        public MainWindow()
        {
            InitializeComponent();

            Rectangle ceil;

            for (var i = 0; i < this.game_grid.RowDefinitions.Count; i++){
                for (var j = 0; j < this.game_grid.ColumnDefinitions.Count; j++) {
                    ceil = new()
                    {
                        Fill = new SolidColorBrush(Colors.Black)
                    };
                    this.game_grid.Children.Add(ceil);
                    Grid.SetColumn(ceil, j);
                    Grid.SetRow(ceil, i);
                }
            }

            this.SpawnFood();
            this.game_grid.Children.Add(SnakeBlock.HEAD.Ceil);
            HttpClient.DefaultProxy = new WebProxy
            {
                Address = new Uri("http://localhost:8000"),
            };

            this.BootstrapSteelseriesGG();
        }

        private (int X, int Y) GetEmptyCeil()
        {
            var not_free = SnakeBlock.GetCoords();

            var x = (from ceil in this.game_grid.Children.OfType<Rectangle>()
                    let coords = (Grid.GetColumn(ceil), Grid.GetRow(ceil))
                    where !not_free.Contains(coords) select coords).ToArray();
            return x[this.random.Next(0, x.Length)];
        }

        private void SpawnFood()
        {
            this.food = new();
            var pos = GetEmptyCeil();
            Grid.SetColumn(this.food.Ceil, pos.X);
            Grid.SetRow(this.food.Ceil, pos.Y);
            this.game_grid.Children.Add(this.food.Ceil);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Direction? direction = null;

            if (e.Key is Key.Left)
                direction = Direction.Left;
            else if (e.Key is Key.Right)
                direction = Direction.Right;
            else if (e.Key is Key.Up)
                direction = Direction.Top;
            else if (e.Key is Key.Down)
                direction = Direction.Bottom;

            if (direction is null)
                return;

            SnakeBlock.HEAD.Move(direction.Value);
            if (this.food is null)
            {
                this.SpawnFood();
                return;
            }

            if (this.food.Position == SnakeBlock.HEAD.Position)
            {
                var tail = SnakeBlock.TAIL;
                var tail_flow = direction.Value;
                tail.Previus = this.food;
                this.food.Next = tail;

                if (tail.Next is not null) {
                    var next_pos = tail.Next.Position;
                    var tail_pos = tail.Position;
                    tail_flow = tail_pos.Y > next_pos.Y ? Direction.Top : tail_pos.Y < next_pos.Y ? Direction.Bottom : tail_pos.X > next_pos.X ? Direction.Left : Direction.Right;
                }

                int final_x = tail.Position.X;
                int final_y = tail.Position.Y;
                if (tail_flow is Direction.Top)
                    final_y++;
                else if (tail_flow is Direction.Bottom)
                    final_y--;
                else if (tail_flow is Direction.Left)
                    final_x++;
                else
                    final_x--;

                if (final_x == -1 && final_y == 9)
                {
                    final_x = 0;
                    final_y = 8;
                }
                else if (final_x == -1 && final_y == 0)
                {
                    final_x = 0;
                    final_y = 1;
                }
                else if (final_x == 0 && final_y == 10)
                {
                    final_x = 1;
                    final_y = 9;
                }
                else if (final_x == 32 && final_y == 9)
                {
                    final_x = 31;
                    final_y = 8;
                }
                else if (final_x == 31 && final_y == 10)
                {
                    final_x = 30;
                    final_y = 9;
                }
                else if (final_x == 31 && final_y == -1)
                {
                    final_x = 30;
                    final_y = 0;
                }
                else if (final_x == 32 && final_y == 0)
                {
                    final_x = 31;
                    final_y = 1;
                }
                else if (final_x == 0 && final_y == -1)
                {
                    final_x = 1;
                    final_y = 0;
                }

                if (final_x != -1 && final_x != 32 && final_y != -1 && final_y != 10)
                {
                    Grid.SetRow(this.food.Ceil, final_y);
                    Grid.SetColumn(this.food.Ceil, final_x);
                }

                this.SpawnFood();
            }
            this.SendToKeyboard();
        }

        private void BootstrapSteelseriesGG()
        {
            var request = new Dictionary<string, object>()
            {
                { "game", "DD004SNAKE" },
                { "game_display_name", "DD004-Snake" },
                { "developer", "DonalDuck004"}
            };
            var serialized = JsonSerializer.Serialize(request);
            var msg = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:60281/game_metadata");
            msg.Content = new StringContent(serialized)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("application/json")
                }
            };

            this.client.Send(msg);
            request = new Dictionary<string, object>()
            {
                { "game", "DD004SNAKE" },
                { "event", "DISPLAY" },
                { "min_value", 0 },
                { "max_value", 100 },
                { "icon_id", null },
                { "handlers", new object[]{
                    new Dictionary<string, object>()
                    {
                        { "datas", new object[] {
                            new Dictionary<string, object>()
                            {
                                { "has-text", false },
                                { "image-data", new int[]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
                            }

                            }
                        },
                        { "device-type",  "screened-128x40" },
                        { "mode",  "screen" },
                        { "zone",  "one" },
                    }
                } },
                { "data_fields", new object[0] }
            };
            serialized = JsonSerializer.Serialize(request);
            msg = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:60281/bind_game_event");
            msg.Content = new StringContent(serialized)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("application/json")
                }
            };

            this.client.Send(msg);
        }

        private void SendToKeyboard()
        {
            RenderTargetBitmap target = new(128, 40, 96, 96, PixelFormats.Pbgra32);
            target.Render(this.game_grid);
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(target));

            var bytes = new byte[128 * 40 / 8];


            using (var stream = new MemoryStream())//new FileStream("a", FileMode.OpenOrCreate, FileAccess.Write))
            {
                encoder.Save(stream);
                var bitmap = new System.Drawing.Bitmap(stream);
                var filler = 0;
                for (int i = 0; i < 40; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        byte tmp = 0;
                        for (int k = 0; k < 8; k++)
                        {
                            var pixel = bitmap.GetPixel(j * 8 + k, i);
                            var gray = (pixel.R + pixel.G + pixel.B) / 3;
                            if (gray == 0)
                                continue;

                            tmp += (byte)(1 << (7 - k));
                        }
                        bytes[filler++] = tmp;
                    }
                }
                stream.Dispose();
                bitmap.Dispose();
            }
            var request = new Dictionary<string, object>()
            {
                { "game", "DD004SNAKE" },
                { "event", "DISPLAY" },
                { "data", new Dictionary<string, object>()
                    {
                        { "value", this.cycle },
                        { "frame",  new Dictionary<string, object>()
                            {
                             {"image-data-128x40", bytes.Select(x => (int)x).ToArray()}
                            }
                        },

                    }
                },

            };
            var serialized = JsonSerializer.Serialize(request);
            var msg = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:60281/game_event");
            msg.Content = new StringContent(serialized)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue("application/json")
                }
            };

            var response = this.client.Send(msg);


            if (++this.cycle == 101)
                this.cycle = 1;
        }
    }
}