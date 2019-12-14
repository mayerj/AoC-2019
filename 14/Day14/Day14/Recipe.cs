using System.Collections.Generic;

namespace Day14
{
    public class Recipe
    {
        private readonly Dictionary<string, int> _inputs = new Dictionary<string, int>();
        private readonly (string result, int amount) _result;

        public Recipe(string[] ingredients, string v2)
        {
            foreach (string ingredient in ingredients)
            {
                string[] parts = ingredient.Trim().Split();

                _inputs[parts[1]] = int.Parse(parts[0]);
            }

            string[] resultParts = v2.Split(new char[0], System.StringSplitOptions.RemoveEmptyEntries);
            _result = (resultParts[1].Trim(), int.Parse(resultParts[0]));

            if (_inputs.Count == 1 && _inputs.ContainsKey("ORE"))
            {
                IsOreOnly = true;
            }
        }

        public Dictionary<string, int> Inputs => _inputs;
        public KeyValuePair<string, int> Outputs => new KeyValuePair<string, int>(_result.result, _result.amount);

        public bool IsOreOnly { get; }
    }
}