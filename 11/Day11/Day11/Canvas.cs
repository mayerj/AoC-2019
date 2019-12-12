using System;
using System.Collections.Generic;
using System.Linq;

namespace Day11
{
    public enum Color
    {
        Black = 0,
        White = 1,
    }

    public class Canvas
    {
        private readonly Dictionary<Point, Color> _data = new Dictionary<Point, Color>();

        public Color GetColor(Point location)
        {
            if (!_data.TryGetValue(location, out var value))
            {
                return Color.Black;
            }

            return value;
        }

        public void Paint(Point location, Color output)
        {
            _data[location] = output;
        }

        public void Print(Point robot, Direction direction)
        {
            int minX = _data.OrderBy(x => x.Key.X).First().Key.X;
            int minY = _data.OrderBy(x => x.Key.Y).First().Key.Y;
            int height = _data.OrderByDescending(x => x.Key.Y).First().Key.Y;
            int width = _data.OrderByDescending(x => x.Key.X).First().Key.X;

            Console.WriteLine("---");
            for (int y = minY; y < height; y++)
            {
                for (int x = minX; x < width; x++)
                {
                    if (robot == new Point(x, y))
                    {
                        char what;
                        switch (direction)
                        {
                            case Direction.Up:
                                what = '^';
                                break;
                            case Direction.Down:
                                what = 'v';
                                break;
                            case Direction.Left:
                                what = '<';
                                break;
                            case Direction.Right:
                                what = '>';
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(direction));

                        }
                        Console.Write(what);
                    }
                    else
                    {
                        char what;
                        switch (GetColor(new Point(x, y)))
                        {
                            case Color.Black:
                                what = '.';
                                break;
                            case Color.White:
                                what = '#';
                                break;
                            default:
                                throw new ArgumentException();
                        }

                        Console.Write(what);
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine($"{_data.Count} Printed");
        }
    }
}