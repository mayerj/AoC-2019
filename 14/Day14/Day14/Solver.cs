using System;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    public class RecursiveSolverProcessor
    {
        private readonly Dictionary<string, Recipe> _recipes;

        private Dictionary<string, int> _excess = new Dictionary<string, int>();
        private int _producedOre;

        public RecursiveSolverProcessor(List<Recipe> list)
        {
            _recipes = list.ToDictionary(x => x.Output.Key);
        }

        public int GetRequiredOre(string desiredType, int desiredAmount)
        {
            GetRequiredInput(desiredType, desiredAmount);

            return _producedOre;
        }

        private void GetRequiredInput(string desiredType, int desiredAmount)
        {
            if (desiredType == "ORE")
            {
                _producedOre += desiredAmount;
                return;
            }

            if (_excess.TryGetValue(desiredType, out int excess))
            {
                if (desiredAmount - excess > 0)
                {
                    _excess.Remove(desiredType);
                    desiredAmount -= excess;
                }
                else
                {
                    _excess[desiredType] -= desiredAmount;
                    return;
                }
            }
            else
            {
                _excess[desiredType] = 0;
            }

            Recipe r = _recipes[desiredType];

            while (desiredAmount > 0)
            {
                foreach (var input in r.Inputs)
                {
                    GetRequiredInput(input.Key, input.Value);
                }

                if (!_excess.TryGetValue(r.Output.Key, out int current))
                {
                    _excess[r.Output.Key] = 0;
                }

                desiredAmount -= r.Output.Value;

                if (desiredAmount < 0)
                {
                    _excess[r.Output.Key] += Math.Abs(desiredAmount);
                }
            }
        }
    }
}