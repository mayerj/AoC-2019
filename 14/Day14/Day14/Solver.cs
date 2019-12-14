using System;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    public class RecursiveSolverProcessor
    {
        public int OreType { get; }
        public int FuelType { get; }

        private readonly Recipe[] _recipes;

        private long[] _excess;
        private long _producedOre = 0;
        private long _maxOre = -1;

        public RecursiveSolverProcessor(List<Recipe> list, Func<string, int> map)
        {
            OreType = map("ORE");
            FuelType = map("FUEL");

            _recipes = new Recipe[list.Max(x => x.Output.Key) + 1];
            _excess = new long[_recipes.Length];
            foreach (Recipe r in list)
            {
                _recipes[r.Output.Key] = r;
            }
        }

        public long GetRequiredOre(int desiredType, long desiredAmount)
        {
            GetRequiredInput(desiredType, desiredAmount);

            return _producedOre;
        }

        private void GetRequiredInput(int desiredType, long desiredAmount)
        {
            if (desiredType == OreType)
            {
                _producedOre += desiredAmount;
                return;
            }

            if (desiredAmount - _excess[desiredType] >= 0)
            {
                desiredAmount -= _excess[desiredType];
                _excess[desiredType] = 0;
            }
            else
            {
                _excess[desiredType] -= desiredAmount;
                return;
            }

            Recipe r = _recipes[desiredType];

            long multiplier = (long)Math.Ceiling(desiredAmount / (double)r.Output.Value);

            foreach (var input in r.Inputs)
            {
                GetRequiredInput(input.Key, input.Value * multiplier);
            }

            desiredAmount -= (r.Output.Value * multiplier);

            if (desiredAmount < 0)
            {
                _excess[r.Output.Key] += Math.Abs(desiredAmount);
            }else if(desiredAmount!=0)
            {

            }
        }

        internal static long GetOreForFuel(List<Recipe> recipes, Func<string, int> map, long count = 1)
        {
            RecursiveSolverProcessor processor = new RecursiveSolverProcessor(recipes, map);

            return processor.GetRequiredOre(processor.FuelType, count);
        }
    }
}