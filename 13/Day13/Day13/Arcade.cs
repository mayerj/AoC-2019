using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day13
{
    public class Arcade
    {
        private long ReadJoystick(IInput inputs)
        {
            return inputs.Next();
        }

        public enum TileId
        {
            Empty = 0,
            Wall = 1,
            Block = 2,
            Paddle = 3,
            Ball = 4,
        }

        private long _score = 0;
        private Dictionary<(long x, long y), TileId> _data = new Dictionary<(long x, long y), TileId>();

        private void ProcessDraw(long x, long y, long tileId)
        {
            if (x == -1 && y == 0)
            {
                _score = tileId;
                return;
            }

            var tile = (TileId)tileId;
            _data[(x, y)] = tile;
        }

        private void Print()
        {
            if (!_output)
            {
                return;
            }

            long maxX = _data.Keys.Max(x => x.x);
            long maxY = _data.Keys.Max(x => x.y);

            Console.Clear();
            Console.WriteLine($"Score: {_score} ({_memory[442]}:{_memory[_memory[442]]}) ({_vm?.GetState()})");
            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    if (_data.TryGetValue((x, y), out var value))
                    {
                        switch (value)
                        {
                            case TileId.Empty:
                                Console.Write(' ');
                                break;
                            case TileId.Wall:
                                Console.Write('W');
                                break;
                            case TileId.Paddle:
                                Console.Write('_');
                                break;
                            case TileId.Block:
                                Console.Write('#');
                                break;
                            case TileId.Ball:
                                Console.Write('O');
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine(' ');
                    }
                }

                Console.WriteLine();
            }
        }

        private Memory _memory;
        private VM _vm;
        private readonly List<long> _initial;
        private readonly bool _debug;
        private readonly bool _output;

        public Arcade(List<long> program, bool debug = false, bool output = true)
        {
            _initial = program;
            _debug = debug;
            _output = output;
        }

        public (long score, int count) Run(IInput inputs)
        {
            Queue<long> instructions = new Queue<long>();

            void Draw()
            {
                if (instructions.Count == 3)
                {
                    ProcessDraw(instructions.Dequeue(), instructions.Dequeue(), instructions.Dequeue());
                }
            }
            int count = 0;
            _score = 0;
            //_data = new Dictionary<(long x, long y), TileId>();

            _memory = new Memory(_initial);
            _vm = new VM(_memory, () => { Print(); count++; return ReadJoystick(inputs); }, x =>
            {
                instructions.Enqueue(x); Draw();
            }, _debug);

            long maxScore = 0;
            while (!_vm.IsHalted)
            {
                maxScore = Math.Max(maxScore, _score);
                _vm.Run();
            }

            return (Math.Max(maxScore, _score), count);
        }

        public long ReadLocation(TileId tileId)
        {
            return _data.Where(x => x.Value == tileId).Select(x => x.Key.x).First();
        }
    }
}