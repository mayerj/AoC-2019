using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day11
{
    public enum Direction
    {
        Up,
        Left,
        Right,
        Down
    }

    public struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point point, (int, int) loc)
        {
            return new Point(point.X + loc.Item1, point.Y + loc.Item2);
        }

        public static bool operator ==(Point lhs, Point rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y;
        }

        public static bool operator !=(Point lhs, Point rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point p)
            {
                return this == p;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(X);
            hashCode.Add(Y);

            return hashCode.ToHashCode();
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public class HullPaintingRobot
    {
        private Point _currentLocation = new Point(0, 0);
        private Direction _direction = Direction.Up;
        private readonly Canvas _canvas;
        private readonly string _input;
        private readonly Memory _memory;
        private readonly VM _vm;

        private bool _isPaint = true;
        private int _iterations = 0;

        public Point Position => _currentLocation;

        public Direction Direction => _direction;

        public HullPaintingRobot(Canvas canvas, string input)
        {
            _canvas = canvas;
            _input = input;

            _memory = new Memory(Parse(input));
            _vm = new VM(_memory, GetColor, ProcessCommand);
        }

        private void ProcessCommand(long output)
        {
            if (_isPaint)
            {
                _canvas.Paint(_currentLocation, (Color)output);
            }
            else
            {
                switch (output)
                {
                    case 0:
                        //turn left
                        switch (_direction)
                        {
                            case Direction.Up:
                                _direction = Direction.Left;
                                break;
                            case Direction.Right:
                                _direction = Direction.Up;
                                break;
                            case Direction.Down:
                                _direction = Direction.Right;
                                break;
                            case Direction.Left:
                                _direction = Direction.Down;
                                break;
                        }
                        break;
                    case 1:
                        //turn right
                        switch (_direction)
                        {
                            case Direction.Up:
                                _direction = Direction.Right;
                                break;
                            case Direction.Right:
                                _direction = Direction.Down;
                                break;
                            case Direction.Down:
                                _direction = Direction.Left;
                                break;
                            case Direction.Left:
                                _direction = Direction.Up;
                                break;
                        }
                        break;
                }

                MoveForward();

                _iterations++;
                if ((_iterations % 50) == 0)
                {
                    //_canvas.Print(_currentLocation, _direction);
                }
            }

            _isPaint = !_isPaint;
        }

        private void MoveForward()
        {
            switch (_direction)
            {
                case Direction.Up:
                    _currentLocation += (0, 1);
                    break;
                case Direction.Left:
                    _currentLocation += (-1, 0);
                    break;
                case Direction.Right:
                    _currentLocation += (1, 0);
                    break;
                case Direction.Down:
                    _currentLocation += (0, -1);
                    break;
            }
        }

        private long GetColor()
        {
            return (long)_canvas.GetColor(_currentLocation);
        }

        private List<long> Parse(string input)
        {
            return input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToList();
        }

        public void Run()
        {
            while (!_vm.IsHalted)
            {
                _vm.Run();
            }
        }
    }
}
