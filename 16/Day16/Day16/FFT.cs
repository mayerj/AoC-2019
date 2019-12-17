using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day16
{
    internal class FFT
    {
        static readonly int[] _basePattern = new[] { 0, 1, 0, -1 };

        private readonly int[] _input;

        public int[] Input => _input;

        public FFT(int[] input)
        {
            _input = input;
        }

        public int[] RunPhase()
        {
            return Phase(_input, _basePattern).ToArray();
        }

        private IEnumerable<int> Phase(int[] input, int[] basePattern)
        {
            for (int i = 0; i < input.Length; i++)
            {
                int sum = 0;
                for (int k = 0; k < input.Length; k++)
                {
                    sum += input[k] * GetMultiplierPattern(i, k + 1);
                }

                var result = Math.Abs(sum) % 10;

                yield return result;
            }
        }

        private static Dictionary<int, int[]> _cache = new Dictionary<int, int[]>();

        private int GetMultiplierPattern(int repeat, int index)
        {
            if (_cache.TryGetValue(repeat, out var value))
            {
                return value[index % value.Length];
            }

            _cache[repeat] = GetMultiplierPattern(repeat).ToArray();

            return GetMultiplierPattern(repeat, index);
        }

        private IEnumerable<int> GetMultiplierPattern(int repeat)
        {
            for (int i = 0; i < _basePattern.Length; i++)
            {
                for (int k = 0; k < repeat + 1; k++)
                {
                    yield return _basePattern[i];
                }
            }
        }
    }
}
