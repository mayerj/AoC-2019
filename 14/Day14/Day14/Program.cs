using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day14
{
    class Program
    {
        static void Main()
        {
            Verify(@"10 ORE => 10 A
1 ORE => 1 B
7 A, 1 B => 1 C
7 A, 1 C => 1 D
7 A, 1 D => 1 E
7 A, 1 E => 1 FUEL", (1, "FUEL"), (31, "ORE"));

            Verify(@"9 ORE => 2 A
8 ORE => 3 B
7 ORE => 5 C
3 A, 4 B => 1 AB
5 B, 7 C => 1 BC
4 C, 1 A => 1 CA
2 AB, 3 BC, 4 CA => 1 FUEL", (1, "FUEL"), (165, "ORE"));
        }

        private static void Verify(string input, (int, string) desired, (int, string) required)
        {
            Solver s = new Solver(Parse(input));

            s.Print();

            SolverProcessor processor = new SolverProcessor(Parse(input));

            int requiredInput = processor.GetRequiredInput(required.Item2, desired.Item2, desired.Item1);

            Debug.Assert(requiredInput == required.Item1);
        }

        private static List<Recipe> Parse(string input)
        {
            return input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => ParseReceipe(x)).ToList();
        }

        private static Recipe ParseReceipe(string data)
        {
            string[] parts = data.Split(new[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);

            return new Recipe(parts[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), parts[1]);
        }
    }
}
