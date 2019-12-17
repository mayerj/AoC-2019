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
                int[] multipliedPattern = GetMultiplier(basePattern, i, input.Length).ToArray();

                int sum = 0;
                for (int k = 0; k < input.Length; k++)
                {
                    sum += input[k] * multipliedPattern[k + 1];
                }

                var result = Math.Abs(sum) % 10;

                yield return result;
            }
        }

        private IEnumerable<int> GetMultiplier(int[] basePattern, int index, int inputLength)
        {
            inputLength++;
            while(inputLength >= 0)
            {
                for (int i = 0; i < basePattern.Length; i++)
                {
                    for (int k = 0; k < index + 1; k++)
                    {
                        inputLength--;
                        yield return basePattern[i];
                    }
                }
            }
        }
    }
}
