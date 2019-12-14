using System;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    public class Recipe
    {
        private readonly Dictionary<int, int> _inputs = new Dictionary<int, int>();
        private readonly KeyValuePair<int, int> _output;

        public Recipe(string[] ingredients, string result, Func<string, int> map)
        {
            foreach (string ingredient in ingredients)
            {
                string[] parts = ingredient.Trim().Split();

                _inputs[map(parts[1])] = int.Parse(parts[0]);
            }

            string[] resultParts = result.Split(new char[0], System.StringSplitOptions.RemoveEmptyEntries);
            _output = new KeyValuePair<int, int>(map(resultParts[1].Trim()), int.Parse(resultParts[0]));

            if (_inputs.Count == 1 && _inputs.ContainsKey(map("ORE")))
            {
                IsOreOnly = true;
            }
        }

        public Recipe(Dictionary<int, int> inputs, KeyValuePair<int, int> output)
        {
            _inputs = inputs;
            _output = output;
        }

        public Dictionary<int, int> Inputs => _inputs;
        public KeyValuePair<int, int> Output => _output;

        public bool IsOreOnly { get; }

        internal Recipe Scale(int scaleFactor)
        {
            return new Recipe(Scale(_inputs, scaleFactor), Scale(_output, scaleFactor));
        }

        private Dictionary<int, int> Scale(Dictionary<int, int> inputs, int scaleFactor)
        {
            return inputs.ToDictionary(x => x.Key, x => x.Value * scaleFactor);
        }

        private KeyValuePair<int, int> Scale(KeyValuePair<int, int> output, int scaleFactor)
        {
            return new KeyValuePair<int, int>(output.Key, output.Value * scaleFactor);
        }
    }
}