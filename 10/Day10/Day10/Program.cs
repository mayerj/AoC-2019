using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            Check(@"
##
##", (0, 0));

            Check(@"
###
###
###", (1, 1));


            Check(@"
.#..#
.....
#####
....#
...##", (3, 4));

            Check(@"
.#..#
.....
#####
....#
...##", @"
.7..7
.....
67775
....7
...87");

            Check(@"
......#.#.
#..#.#....
..#######.
.#.#.###..
.#..#.....
..#....#.#
#..#....#.
.##.#..###
##...#..#.
.#....####", (5, 8));

            Check(@"#.#...#.#.
.###....#.
.#....#...
##.#.#.#.#
....#.#.#.
.##..###.#
..#...##..
..##....##
......#...
.####.###.", (1, 2));

            Check(@".#..#..###
####.###.#
....###.#.
..###.##.#
##.##.#.#.
....###..#
..#.#..#.#
#..#.#.###
.##...##.#
.....#.#..", (6, 3));

            Check(@".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##", (11, 13));

            var optimal = FindOptimal(Parse(@"###..#########.#####.
.####.#####..####.#.#
.###.#.#.#####.##..##
##.####.#.###########
###...#.####.#.#.####
#.##..###.########...
#.#######.##.#######.
.#..#.#..###...####.#
#######.##.##.###..##
#.#......#....#.#.#..
######.###.#.#.##...#
####.#...#.#######.#.
.######.#####.#######
##.##.##.#####.##.#.#
###.#######..##.#....
###.##.##..##.#####.#
##.########.#.#.#####
.##....##..###.#...#.
#..#.####.######..###
..#.####.############
..##...###..#########"));

            Console.WriteLine(optimal);
        }

        private static void Check(string input, (int x, int y) expected)
        {
            Map map = Parse(input);

            var optimal = FindAll(map);

            Debug.Assert(optimal.OrderByDescending(x => x.Value).First().Key == expected);
        }

        private static void Check(string input, string expected)
        {
            Map map = Parse(input);

            var optimal = FindAll(map);

            var expectedData = ParseExpected(expected);

            foreach (var data in optimal)
            {
                Debug.Assert(data.Value == expectedData[data.Key.y][data.Key.x]);
            }
        }

        private static Dictionary<(int x, int y), int> FindAll(Map map)
        {
            Dictionary<(int x, int y), int> locations = new Dictionary<(int x, int y), int>();
            foreach (var location in GetAsteroids(map))
            {
                locations[location] = GetVisibleAsteroids(map, location);
            }

            return locations;
        }
        private static ((int x, int y), int visibleAsteroids) FindOptimal(Map map)
        {
            Dictionary<(int x, int y), int> locations = new Dictionary<(int x, int y), int>();
            foreach (var location in GetAsteroids(map))
            {
                locations[location] = GetVisibleAsteroids(map, location);
            }

            var optimal = FindAll(map).OrderByDescending(x => x.Value).First();

            return (optimal.Key, optimal.Value);
        }

        private static int GetVisibleAsteroids(Map map, (int x, int y) location)
        {
            HashSet<double> angles = new HashSet<double>();
            foreach (var asteroid in GetAsteroids(map))
            {
                if (asteroid == location)
                {
                    continue;
                }

                angles.Add(GetAngle(asteroid, location));
            }

            return angles.Count;
        }

        private static double GetAngle((int x, int y) asteroid, (int x, int y) location)
        {
            var angle = Math.Atan2(location.y - asteroid.y, location.x - asteroid.x);

            return angle;
        }

        private static IEnumerable<(int x, int y)> GetAsteroids(Map map)
        {
            var bounds = map.GetBounds();

            for (int x = 0; x < bounds.width; x++)
            {
                for (int y = 0; y < bounds.height; y++)
                {
                    if (map.IsAsteroid((x, y)))
                    {
                        yield return (x, y);
                    }
                }
            }
        }

        private static Map Parse(string input)
        {
            string[] data = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Map map = new Map(data.Select(x => x.Select(y => y == '.' ? false : true).ToArray()).ToArray());

            return map;
        }

        private static int[][] ParseExpected(string input)
        {
            string[] data = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return data.Select(x => x.Select(y => y == '.' ? 0 : int.Parse(y.ToString())).ToArray()).ToArray();
        }
    }

    public class Map
    {
        private bool[][] _map;

        public Map(bool[][] map)
        {
            _map = map;
        }

        public (int height, int width) GetBounds() => (_map[0].Length, _map.Length);

        public bool IsAsteroid((int x, int y) location) => IsInBounds(location) && _map[location.y][location.x];

        internal bool IsInBounds((int x, int y) location)
        {
            var bound = GetBounds();
            return location.x >= 0 && location.y >= 0 && location.x < bound.width && location.y < bound.height;
        }
    }
}
