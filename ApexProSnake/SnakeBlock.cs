using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ApexProSnake
{
    class SnakeBlock
    {
        public static SolidColorBrush COLOR { get; } = new(Colors.White);

        public static SnakeBlock HEAD { get; } = new();
        public static SnakeBlock TAIL {
            get
            {
                var block = SnakeBlock.HEAD;
                while (block.Previus is not null) 
                    block = block.Previus;
                return block;
            }
        }

        public SnakeBlock? Next { get; set; }
        public SnakeBlock? Previus { get; set; }

        public Rectangle Ceil { get; init; }

        public (int X, int Y) Position => (Grid.GetColumn(this.Ceil), Grid.GetRow(this.Ceil));

        public SnakeBlock()
        {
            this.Ceil = new Rectangle()
            {
                Fill = SnakeBlock.COLOR
            };

            Grid.SetZIndex(this.Ceil, 1);
        }

        public void Move(Direction direction, bool _first = true)
        {
            var x = Grid.GetColumn(this.Ceil);
            var y = Grid.GetRow(this.Ceil);

            var new_y = y;
            var new_x = x;

            if (direction is Direction.Left)
                new_x--;
            else if (direction is Direction.Right)
                new_x++;
            else if (direction is Direction.Bottom)
                new_y++;
            else
                new_y--;

            if (new_y == -1 || new_y == 10 || new_x == -1 || new_x == 32)
            {
                if (_first)
                    return;

                this.Next!.Previus = this.Previus;
                if (this.Previus is not null)
                    this.Previus.Next = this.Next;
                ((Grid)this.Ceil.Parent).Children.Remove(this.Ceil);
                return;
            }

            Grid.SetColumn(this.Ceil, new_x);
            Grid.SetRow(this.Ceil, new_y);

            if (this.Previus is not null)
            {
                var previus_pos = this.Previus.Position;

                if (direction is Direction.Right && previus_pos.Y != y)
                    direction = previus_pos.Y > y ? Direction.Top : Direction.Bottom;
                else if (direction is Direction.Top && previus_pos.X != x)
                    direction = previus_pos.X > x ? Direction.Left : Direction.Right;
                else if (direction is Direction.Bottom && previus_pos.X != x)
                    direction = previus_pos.X > x ? Direction.Left : Direction.Right;
                else if(direction is Direction.Left && previus_pos.Y != y)
                    direction = previus_pos.Y > y ? Direction.Top : Direction.Bottom;

                this.Previus.Move(direction, false);
            }
        }

        public static HashSet<(int, int)> GetCoords()
        {
            HashSet<(int, int)> rt = new();

            var block = SnakeBlock.HEAD;
            do
            {
                rt.Add((Grid.GetColumn(block.Ceil), Grid.GetRow(block.Ceil)));
                block = block.Previus;
            } while (block is not null);
            return rt;
        }

    }
}