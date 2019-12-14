using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Day13
{
    public class InputGenerator
    {
        private readonly object _lock1 = new object();

        //private Dictionary<int[], long> _inputs = new Dictionary<int[], long>(new ArrayComparer());

        private readonly BlockingCollection<int[]> _toCheck;

        public Input GetInputs()
        {
            int[] data = _toCheck.Take();

            return new Input(data);
        }

        internal void Report(Input inputs, long score)
        {
            if (inputs.Overran)
            {
                //lock (_lock1)
                {
                    //_inputs[inputs.Inputs] = score;
                    Mutate(inputs.Inputs);
                }
                return;
            }
            else
            {
            }
            //else if (inputs.Inputs.Length == inputs.Index)
            //{
            //    lock (_lock1)
            //    {
            //        _inputs[inputs.Inputs] = score;
            //    }
            //}
        }

        public InputGenerator()
        {
            _toCheck = new BlockingCollection<int[]>(new ConcurrentStack<int[]>());
            Mutate(Array.Empty<int>());
        }

        private void Mutate(int[] data)
        {
            for (int i = -1; i < 2; i++)
            {
                var newInput = data.Append(i).ToArray();

                //if (_inputs.ContainsKey(newInput))
                //{
                //    return;
                //}
                _toCheck.Add(newInput);
            }
        }
    }

    internal class ArrayComparer : IEqualityComparer<int[]>
    {
        public bool Equals([AllowNull] int[] x, [AllowNull] int[] y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.SequenceEqual(y);
        }

        public int GetHashCode([DisallowNull] int[] obj)
        {
            return obj.Sum();
        }
    }

    public class Input
    {
        private int _index = 0;
        private bool _overran = false;
        private readonly int[] _inputs;

        public override string ToString()
        {
            return string.Join(',', _inputs);
        }

        public Input(int[] inputs)
        {
            _inputs = inputs;
        }

        public bool Overran => _overran;
        public int[] Inputs => _inputs;

        public int Index => _index;

        public long Next()
        {
            if (_inputs.Length > _index)
            {
                return _inputs[_index++];
            }

            _overran = true;
            return 0;
        }
    }
}
