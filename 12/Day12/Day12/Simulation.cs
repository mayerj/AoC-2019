using System;
using System.Collections.Generic;
using System.Linq;

namespace Day12
{
    internal class Simulation
    {
        private readonly List<Moon> _moons;

        public Simulation(List<Moon> moons)
        {
            _moons = moons;
        }

        public int Energy { get { return _moons.Sum(x => x.GetEnergy()); } }

        public void Run(int steps)
        {
            Output(0);

            for (int i = 0; i < steps; i++)
            {
                RunStep(i + 1);
            }
        }

        private void RunStep(int step)
        {
            IEnumerable<(Moon one, Moon two)> pairs = GetPairs();

            foreach (var pair in pairs)
            {
                Mutate(pair.one, pair.two);
            }

            Advance();

            Output(step);
        }

        private void Output(int step)
        {
            Console.WriteLine($"--- Step {step} ---");
            foreach (var moon in _moons)
            {
                Console.WriteLine(moon.ToString());
            }
        }

        private void Advance()
        {
            foreach (Moon moon in _moons)
            {
                moon.Move();
            }
        }

        private void Mutate(Moon one, Moon two)
        {
            if (one.X > two.X)
            {
                one.DX -= 1;
                two.DX += 1;
            }
            else if (one.X < two.X)
            {
                one.DX += 1;
                two.DX -= 1;
            }

            if (one.Y > two.Y)
            {
                one.DY -= 1;
                two.DY += 1;
            }
            else if (one.Y < two.Y)
            {
                one.DY += 1;
                two.DY -= 1;
            }

            if (one.Z > two.Z)
            {
                one.DZ -= 1;
                two.DZ += 1;
            }
            else if (one.Z < two.Z)
            {
                one.DZ += 1;
                two.DZ -= 1;
            }
        }

        private IEnumerable<(Moon one, Moon two)> GetPairs()
        {
            for (int i = 0; i < _moons.Count; i++)
            {
                for (int j = i + 1; j < _moons.Count; j++)
                {
                    yield return (_moons[i], _moons[j]);
                }
            }
        }
    }
}