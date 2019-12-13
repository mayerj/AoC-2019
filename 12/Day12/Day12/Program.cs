using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day12
{
    class Program
    {
        static void Main(string[] args)
        {
            Verify(@"<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>", 10, 179);

            Verify(@"<x=-8, y=-10, z=0>
<x=5, y=5, z=10>
<x=2, y=-7, z=3>
<x=9, y=-8, z=-3>", 100, 1940);

            Console.WriteLine($"{GetEnergy(Input, 1000)}");

            Console.WriteLine("--- Part 2 ---");

            VerifyCycle(@"<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>", 2772);

            VerifyCycle(@"<x=-8, y=-10, z=0>
<x=5, y=5, z=10>
<x=2, y=-7, z=3>
<x=9, y=-8, z=-3>", 4686774924);
        }

        private static void VerifyCycle(string input, ulong cycleSteps)
        {
            List<Moon> moons = Parse(input);

            Simulation s = new Simulation(moons);

            ulong steps = 0;
            int state = s.GetState();
            while (steps <= cycleSteps)
            {
                steps++;
                s.Run(1);

                if (state == s.GetState())
                {
                    break;
                }
            }

            Debug.Assert(steps == cycleSteps);
        }

        private static void Verify(string input, int steps, int expectedEnergy)
        {
            int energy = GetEnergy(input, steps);

            Debug.Assert(energy == expectedEnergy);
        }

        private static int GetEnergy(string input, int steps)
        {
            List<Moon> moons = Parse(input);

            Simulation s = new Simulation(moons);

            s.Run(steps);
            return s.Energy;
        }

        private static List<Moon> Parse(string input)
        {
            int i = 0;
            return input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => ParseMoon(x, i++)).ToList();
        }

        private static Moon ParseMoon(string moon, int index)
        {
            moon = moon.Trim().Trim(new[] { '<', '>' });

            var data = moon.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x.Split(new[] { '=' })[1])).ToArray();

            Moon m = new Moon(index, data[0], data[1], data[2]);

            return m;
        }

        private static string Input = @"<x=14, y=15, z=-2>
<x=17, y=-3, z=4>
<x=6, y=12, z=-13>
<x=-2, y=10, z=-8>";
    }
}
