using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Day17
{
    class VacuumRobot
    {
        private readonly Memory _memory;
        private readonly List<StringBuilder> _outputs;
        private readonly VM _vm;
        private List<long> _data;
        private long _last;
        private readonly bool _live;
        private bool _failed = false;

        public VacuumRobot(string input, bool live)
        {
            _live = live;
            _memory = new Memory(input.Split(new[] { ',' }).Select(x => long.Parse(x)).ToList());

            _outputs = new List<StringBuilder>() { new StringBuilder() };

            void Flush()
            {
                if (!_live)
                {
                    return;
                }

                Console.Clear();
                foreach (var str in _outputs)
                {
                    Console.WriteLine(str);
                }

                Thread.Sleep(100);
            }

            _last = 0;
            int line = 0;
            int i = 0;
            _vm = new VM(_memory, () =>
            {
                if (i + 1 == _data.Count) { line = 0; }
                return _data[i++];

            }, (x) =>
            {
                _last = x;
                if (x == 10)
                {
                    //Thread.Sleep(10);
                    line += 1;
                    if (line == 66)
                    {
                        Flush();
                        line = 0;
                    }

                    _outputs.Add(new StringBuilder());
                    return;
                }

                if ('X' == (char)x)
                {
                    _failed = true;
                }

                _outputs.Last().Append((char)x);
            });
        }

        internal (long score, bool failed) Run(string movementRoutine, string a, string b, string c)
        {
            _data = new List<long>();
            foreach (var routine in new[] { movementRoutine, a, b, c })
            {
                string[] data = routine.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                char[][] converted = Convert(data).ToArray();

                List<long> local = new List<long>();
                if (converted.Length != 0)
                {
                    local.AddRange(converted[0].Select(x => (long)x));
                    for (int i = 1; i < converted.Length; i++)
                    {
                        local.Add(',');
                        local.AddRange(converted[i].Select(x => (long)x));
                    }
                }
                Debug.Assert(local.Count <= 20);

                local.Add(10);

                _data.AddRange(local);
            }

            _data.Add(_live ? 'y' : 'n');
            _data.Add(10);

            while (!_vm.IsHalted)
            {
                _vm.Run();
            }

            return (_last, _failed);
        }

        private IEnumerable<char[]> Convert(string[] data)
        {
            foreach (var str in data)
            {
                if (char.IsNumber(str[0]))
                {
                    yield return str.ToCharArray();
                }
                else
                {
                    yield return str.ToCharArray();
                }
            }
        }
    }
}
