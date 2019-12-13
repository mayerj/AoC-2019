using System;
using System.Diagnostics;

namespace Day12
{
    [DebuggerDisplay("{Index}")]
    public class Moon
    {
        public int Index { get; }

        private int _x;
        private int _y;
        private int _z;

        public Moon(int index, int x, int y, int z)
        {
            Index = index;
            _x = x;
            _y = y;
            _z = z;
        }

        public override string ToString()
        {
            return $"pos=<x={_x},   y={_y}, z={_z}>, vel=<x={DX},   y={DY}, z={DZ}>";
        }

        public int GetEnergy()
        {
            var potential = Math.Abs(_x) + Math.Abs(_y) + Math.Abs(_z);
            var kinetic = Math.Abs(DX) + Math.Abs(DY) + Math.Abs(DZ);

            return potential * kinetic;
        }

        public void Move()
        {
            _x += DX;
            _y += DY;
            _z += DZ;
        }

        public int X => _x;
        public int Y => _y;
        public int Z => _z;
        public int DX { get; set; }
        public int DY { get; set; }
        public int DZ { get; set; }

        public int GetState()
        {
            HashCode hashCode = new HashCode();

            hashCode.Add(_x);
            hashCode.Add(_y);
            hashCode.Add(_z);

            hashCode.Add(DX);
            hashCode.Add(DY);
            hashCode.Add(DZ);

            return hashCode.ToHashCode();
        }
    }
}