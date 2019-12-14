using System;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    internal class Solver
    {
        private readonly IReadOnlyList<Recipe> _recipes;
        //private readonly int[,] _matrix;

        private readonly IReadOnlyDictionary<string, int> _excess;
        private readonly IReadOnlyDictionary<string, int> _total;

        public int Steps { get; }

        public Solver(IReadOnlyList<Recipe> recipes) : this(recipes, new Dictionary<string, int>(ZeroState(recipes)), new Dictionary<string, int>(ZeroState(recipes)), 0)
        { }

        private static IDictionary<string, int> ZeroState(IReadOnlyList<Recipe> recipes)
        {
            Dictionary<string, int> start = new Dictionary<string, int>();
            var all = new List<string>(new HashSet<string>(recipes.SelectMany(x => x.Inputs.Keys).Concat(recipes.Select(x => x.Outputs.Key))));
            foreach (var v in all)
            {
                start[v] = 0;
            }

            return start;

        }

        public Solver(IReadOnlyList<Recipe> recipes, Dictionary<string, int> excess, Dictionary<string, int> total, int steps)
        {
            _recipes = recipes;
            _excess = excess;
            _total = total;
            Steps = steps;

            //_matrix = new int[_recipes.Count, _all.Count];
            //
            //for (int i = 0; i < _recipes.Count; i++)
            //{
            //    foreach (var input in _recipes[i].Inputs)
            //    {
            //        int index = _all.IndexOf(input.Key);
            //
            //        _matrix[i, index] = input.Value;
            //    }
            //
            //    _matrix[i, _all.IndexOf(_recipes[i].Outputs.Key)] = _recipes[i].Outputs.Value;
            //}
        }

        public void Print()
        {
            var all = new List<string>(new HashSet<string>(_recipes.SelectMany(x => x.Inputs.Keys).Concat(_recipes.Select(x => x.Outputs.Key))));

            foreach (var data in all)
            {
                Console.WriteLine($"\"{data}\"");
            }

            //for (int y = 0; y < _all.Count; y++)
            //{
            //    Console.Write($"{_all[y]}\t");
            //    for (int x = 0; x < _recipes.Count; x++)
            //    {
            //        Console.Write(_matrix[x, y]);
            //        Console.Write('\t');
            //    }
            //    Console.WriteLine();
            //}
        }

        internal bool Has(string desiredType, int desiredAmount)
        {
            if (_excess.TryGetValue(desiredType, out var amt))
            {
                return amt >= desiredAmount;
            }

            return false;
        }

        internal int Amount(string inputType)
        {
            return _excess[inputType];
        }

        internal IEnumerable<Solver> Mutate()
        {
            List<Recipe> ores = new List<Recipe>();
            for (int i = 0; i < _recipes.Count; i++)
            {
                if (_recipes[i].IsOreOnly)
                {
                    ores.Add(_recipes[i]);
                    continue;
                }

                if (CanTransform(_recipes[i]))
                {
                    yield return Transform(_recipes[i]);
                }
            }

            foreach (Recipe ore in ores)
            {
                yield return Transform(ore);
            }
        }

        private bool CanTransform(Recipe recipe)
        {
            foreach (var input in recipe.Inputs)
            {
                if (input.Key != "ORE")
                {
                    if (!Has(input.Key, input.Value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private Solver Transform(Recipe recipe)
        {
            var excess = new Dictionary<string, int>(_excess);

            foreach (var input in recipe.Inputs)
            {
                excess[input.Key] -= input.Value;
            }

            excess[recipe.Outputs.Key] += recipe.Outputs.Value;

            var total = new Dictionary<string, int>(_total);



            return new Solver(_recipes, excess, total, Steps + 1);
        }

        internal string Show()
        {
            return string.Join(", ", _excess.Select(x => $"{x.Key}: {x.Value}"));
        }
    }

    public class SolverProcessor
    {
        private readonly List<Recipe> _recipes;

        private (Solver best, int minOre)? _best = null;

        public SolverProcessor(List<Recipe> list)
        {
            _recipes = list;
        }

        internal int GetRequiredInput(string inputType, string desiredType, int desiredAmount)
        {
            return GetRequiredInput(new Solver(_recipes), inputType, desiredType, desiredAmount);
        }

        private int GetRequiredInput(Solver startState, string inputType, string desiredType, int desiredAmount)
        {
            SortedList<int, List<Solver>> _solvers = new SortedList<int, List<Solver>>();

            _solvers.Add(0, new List<Solver> { startState });

            int i = 0;
            while (_solvers.Count != 0)
            {
                var minOre = _solvers.Keys.Max();
                List<Solver> currentList = _solvers[minOre];
                if (currentList.Count == 0)
                {
                    _solvers.Remove(minOre);
                    continue;
                }

                currentList.Sort(MinOre);
                var current = currentList[0];
                currentList.RemoveAt(0);

                if ((i++ % 50) == 0)
                {
                    Console.WriteLine(current.Show());
                }

                if (_best != null)
                {
                    currentList.RemoveAll(x => Math.Abs(x.Amount("ORE")) > _best.Value.minOre);
                }

                if (current.Has(desiredType, desiredAmount))
                {
                    int ore = Math.Abs(current.Amount(inputType));

                    if (_best == null || _best.Value.minOre > ore)
                    {
                        _best = (current, ore);
                        continue;
                    }
                }

                foreach (var solver in current.Mutate())
                {
                    var amt = solver.Amount(inputType);

                    if (_best != null && _best.Value.minOre <= Math.Abs(amt))
                    {
                        continue;
                    }

                    if (!_solvers.TryGetValue(amt, out var list))
                    {
                        _solvers[amt] = list = new List<Solver>();
                    }

                    list.Add(solver);
                }
            }

            return _best.Value.minOre;
        }

        private int MinOre(Solver x, Solver y)
        {
            var ore1 = x.Amount("ORE");
            var ore2 = y.Amount("ORE");

            var delta = Math.Abs(ore1) - Math.Abs(ore2);

            if (delta != 0)
            {
                return delta;
            }

            return x.Steps - y.Steps;
        }
    }
}