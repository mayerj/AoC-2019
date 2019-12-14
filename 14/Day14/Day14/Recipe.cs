using System.Collections.Generic;

namespace Day14
{
    public class Recipe
    {
        private readonly Dictionary<string, int> _inputs = new Dictionary<string, int>();
        private readonly KeyValuePair<string, int> _output;

        public Recipe(string[] ingredients, string result)
        {
            foreach (string ingredient in ingredients)
            {
                string[] parts = ingredient.Trim().Split();

                _inputs[parts[1]] = int.Parse(parts[0]);
            }

            string[] resultParts = result.Split(new char[0], System.StringSplitOptions.RemoveEmptyEntries);
            _output = new KeyValuePair<string, int>(resultParts[1].Trim(), int.Parse(resultParts[0]));

            if (_inputs.Count == 1 && _inputs.ContainsKey("ORE"))
            {
                IsOreOnly = true;
            }
        }

        public Recipe(Dictionary<string, int> inputs, KeyValuePair<string, int> output)
        {
            _inputs = inputs;
            _output = output;
        }

        public Dictionary<string, int> Inputs => _inputs;
        public KeyValuePair<string, int> Output => _output;

        public bool IsOreOnly { get; }
    }
}