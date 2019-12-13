using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day12
{
    public class Vector
    {
        public Vector(string label, int coordinate, int speed)
        {
            Label = label;
            Coordinate = coordinate;
            Speed = speed;
        }

        public string Label { get; }
        public int Coordinate { get; set; }
        public int Speed { get; set; }
    }

    [DebuggerDisplay("{Index}")]
    public class Moon
    {
        public int Index { get; }

        private readonly Vector[] _vectors;

        public Moon(int index, params Vector[] vectors)
        {
            Index = index;
            _vectors = vectors;
        }

        public Moon(Moon other)
            : this(other.Index, other._vectors.Select(x => new Vector(x.Label, x.Coordinate, x.Speed)).ToArray())
        {
        }

        public IReadOnlyList<Vector> Vectors => _vectors;

        public override string ToString()
        {
            string position = $"pos=<{_vectors.Select(x => $"{x.Label}={x.Coordinate}")}>";

            string velocity = $"vel=<{_vectors.Select(x => $"{x.Label}={x.Speed}")}>";

            return $"{position} {velocity}";
        }

        public int GetEnergy()
        {
            int potential = 0;
            int kinetic = 0;
            foreach (var c in _vectors)
            {
                potential += Math.Abs(c.Coordinate);
                kinetic += Math.Abs(c.Speed);
            }

            return potential * kinetic;
        }

        public void Move()
        {
            foreach (var c in _vectors)
            {
                c.Coordinate += c.Speed;
            }
        }

        public int GetState()
        {
            HashCode hashCode = new HashCode();

            foreach (var c in _vectors)
            {
                hashCode.Add(c.Coordinate);
                hashCode.Add(c.Speed);
            }

            return hashCode.ToHashCode();
        }

        public void Mutate(Moon other)
        {
            for (int i = 0; i < _vectors.Length; i++)
            {
                if (_vectors[i].Coordinate > other._vectors[i].Coordinate)
                {
                    _vectors[i].Speed -= 1;
                    other._vectors[i].Speed += 1;
                }
                else if (_vectors[i].Coordinate < other._vectors[i].Coordinate)
                {
                    _vectors[i].Speed += 1;
                    other._vectors[i].Speed -= 1;
                }
            }
        }
    }
}