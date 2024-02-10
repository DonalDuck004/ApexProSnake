
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ApexProSnake
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random random = new();
        private Gamesense.Gamesense gamesense = new();
        private SnakeBlock food;
        private int cycle = 1;
        private byte[] image_buff = new byte[128 * 40 / 8];

        public MainWindow()
        {
            InitializeComponent();

            Rectangle ceil;
            var Fill = new SolidColorBrush(Colors.Black);

            for (var i = 0; i < this.game_grid.RowDefinitions.Count; i++){
                for (var j = 0; j < this.game_grid.ColumnDefinitions.Count; j++) {
                    ceil = new()
                    {
                        Fill = Fill
                    };
                    this.game_grid.Children.Add(ceil);
                    Grid.SetColumn(ceil, j);
                    Grid.SetRow(ceil, i);
                }
            }

            this.SpawnFood();
            this.game_grid.Children.Add(SnakeBlock.HEAD.Ceil);
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

            if (this.food.Position == SnakeBlock.HEAD.Position)
            {
                var tail = SnakeBlock.TAIL;
                var tail_flow = direction.Value;


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

                if (final_x != -1 && final_x != 32 && final_y != -1 && final_y != 10){
                    Grid.SetRow(this.food.Ceil, final_y);
                    Grid.SetColumn(this.food.Ceil, final_x);

                    tail.Previus = this.food;
                    this.food.Next = tail;
                }
                else
                    this.game_grid.Children.Remove(this.food.Ceil);
                // There is no gameover screen

                this.SpawnFood();
            }
            this.SendToKeyboard();
        }

        private void BootstrapSteelseriesGG()
        {
            var meta = new Gamesense.GameMetadata() { 
                Game = "DD004SNAKE", 
                GameDisplayName = "DD004-Snake",
                Developer = "DonalDuck004"
            };
            this.gamesense.GameMetadata(meta);

            var bind_game_event = new Gamesense.BindGameEvent<Gamesense.BindGameEventHandlerDataDisplay>()
            {
                Game = "DD004SNAKE",
                Event = "DISPLAY",
                Handlers = new []{
                    new Gamesense.BindGameEventHandler<Gamesense.BindGameEventHandlerDataDisplay>()
                    {
                        Datas = new[]
                        {
                            new Gamesense.BindGameEventHandlerDataDisplay()
                            {
                                HasText = false,
                                ImageData = this.image_buff
                            }
                        },
                        DeviceType = "screened-128x40",
                        Mode = "screen",
                        Zone = "one",
                    }
                }
            };
            this.gamesense.BindGameEvent(bind_game_event);


            this.SendToKeyboard();
        }

        private unsafe void SendToKeyboard()
        {
            Array.Clear(this.image_buff);

            var block = SnakeBlock.HEAD;
            byte b1, b2, b3, b4;
            int idx1, idx2, idx3, idx4, row;
            (int X, int Y) pos;

            do
            {
                pos = block.Position;
                pos.Y *= 4;
                pos.X *= 4;
                
                b1 = (byte)(1 << (7 - (pos.X  % 8)));
                b2 = (byte)(1 << (7 - ((pos.X + 1) % 8)));
                b3 = (byte)(1 << (7 - ((pos.X + 2) % 8)));
                b4 = (byte)(1 << (7 - ((pos.X + 3) % 8)));

                row = pos.Y * 16;
                idx1 = pos.X / 8 + row;
                idx2 = ((pos.X + 1) / 8) + row;
                idx3 = ((pos.X + 2) / 8) + row;
                idx4 = ((pos.X + 3) / 8) + row;

                this.image_buff[idx1] |= b1;
                this.image_buff[idx2] |= b2;
                this.image_buff[idx3] |= b3;
                this.image_buff[idx4] |= b4;

                this.image_buff[idx1 + 16] |= b1;
                this.image_buff[idx2 + 16] |= b2;
                this.image_buff[idx3 + 16] |= b3;
                this.image_buff[idx4 + 16] |= b4;

                this.image_buff[idx1 + 32] |= b1;
                this.image_buff[idx2 + 32] |= b2;
                this.image_buff[idx3 + 32] |= b3;
                this.image_buff[idx4 + 32] |= b4;

                this.image_buff[idx1 + 48] |= b1;
                this.image_buff[idx2 + 48] |= b2;
                this.image_buff[idx3 + 48] |= b3;
                this.image_buff[idx4 + 48] |= b4;

                if (block.Previus is null && !object.ReferenceEquals(block, this.food))
                    block = this.food; // force food printing
                else
                    block = block.Previus;


            } while (block is not null);

            var game_event = new Gamesense.GameEvent<Gamesense.GameEventDisplayData>()
            {
                Game = "DD004SNAKE",
                Event = "DISPLAY",
                Data = new Gamesense.GameEventDisplayData()
                {
                    Value = this.cycle,
                    Frame = new Dictionary<string, IEnumerable<byte>>()
                    {
                        { "image-data-128x40", this.image_buff }
                    }
                }
            };
            
            this.gamesense.GameEvent(game_event);

            if (++this.cycle == 101)
                this.cycle = 1;
        }
    }
}