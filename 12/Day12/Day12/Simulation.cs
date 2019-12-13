using System;
using System.Collections.Generic;
using System.Linq;

namespace Day12
{
    internal class Simulation
    {
        private readonly List<Moon> _moons;
        private readonly bool _debug;
        private ulong _runSteps = 0;

        public Simulation(List<Moon> moons, bool debug = false)
        {
            _moons = moons;
            _debug = debug;
        }

        public Simulation(Simulation other)
            : this(new List<Moon>(other._moons.Select(x => new Moon(x))), other._debug)
        {
            _runSteps = other._runSteps;
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
            _runSteps++;

            IEnumerable<(Moon one, Moon two)> pairs = GetPairs();

            foreach (var (one, two) in pairs)
            {
                Mutate(one, two);
            }

            Advance();

            Output(step);
        }

        private void Output(int step)
        {
            if (!_debug) { return; }
            Console.WriteLine($"--- Step {step} ---");
            foreach (var moon in _moons)
            {
                Console.WriteLine(moon.ToString());
            }
        }

        internal int GetState()
        {
            HashCode hashCode = new HashCode();
            foreach (var moon in _moons)
            {
                hashCode.Add(moon.GetState());
            }

            return hashCode.ToHashCode();
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
            one.Mutate(two);
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