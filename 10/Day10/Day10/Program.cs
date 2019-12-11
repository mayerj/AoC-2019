using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day10
{
    static class Program
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

            string bigInput = @".#..##.###...#######
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
###.##.####.##.#..##";

            Check(bigInput, (11, 13));

            string input = @"###..#########.#####.
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
..##...###..#########";

            var optimal = FindOptimal(Parse(input));

            Console.WriteLine(optimal);

            Console.WriteLine("Part 2");

            CheckVaporizationIndex(@"
.#.
###
.#.", (1, 1), null, (0, (1, 0)), (1, (2, 1)), (2, (1, 2)), (3, (0, 1)));

            CheckVaporizationIndex(bigInput, (11, 13), null, (0, (11, 12)), (1, (12, 1)), (2, (12, 2)), (9, (12, 8)), (199, (8, 2)));

            CheckVaporizationIndex(input, (11, 11), (199, ((int x, int y) point) => Console.WriteLine((point.x * 100) + point.y)));
        }

        private static void CheckVaporizationIndex(string input, (int x, int y) startingLocation, (int index, Action<(int x, int y)> action)? callback, params (int index, (int x, int y) location)[] expected)
        {
            Map map = Parse(input);

            var given = map.Vaporize(startingLocation).ToArray();

            foreach (var expect in expected)
            {
                Debug.Assert(given[expect.index] == expect.location);
            }

            if (callback.HasValue)
            {
                callback.Value.action(given[callback.Value.index]);
            }
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
                locations[location] = GetVisibleAsteroids(map, location).Count;
            }

            return locations;
        }
        private static ((int x, int y), int visibleAsteroids) FindOptimal(Map map)
        {
            Dictionary<(int x, int y), int> locations = new Dictionary<(int x, int y), int>();
            foreach (var location in GetAsteroids(map))
            {
                locations[location] = GetVisibleAsteroids(map, location).Count;
            }

            var optimal = FindAll(map).OrderByDescending(x => x.Value).First();

            return (optimal.Key, optimal.Value);
        }

        private static Dictionary<double, (int x, int y)> GetVisibleAsteroids(Map map, (int x, int y) location)
        {
            Dictionary<double, (int x, int y)> visible = new Dictionary<double, (int x, int y)>();
            foreach (var asteroid in GetAsteroids(map))
            {
                if (asteroid == location)
                {
                    continue;
                }

                double angle = GetAngle(asteroid, location);

                if (visible.TryGetValue(angle, out var current))
                {
                    if (Distance(location, current) < Distance(location, asteroid))
                    {
                        continue;
                    }
                }

                visible[angle] = asteroid;
            }

            return visible;
        }

        private static double Distance((int x, int y) location, (int x, int y) location2)
        {
            return Math.Sqrt(Math.Pow((location2.x - location.x), 2) + Math.Pow((location2.y - location.y), 2));
        }

        private static double GetAngle((int x, int y) asteroid, (int x, int y) location)
        {
            var angle = Math.Atan2(location.x - asteroid.x, location.y - asteroid.y);

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

        public static IEnumerable<(int x, int y)> Vaporize(this Map map, (int x, int y) startingLocation)
        {
            //map.Output(startingLocation);

            while (GetAsteroids(map).Where(x => x != startingLocation).Any())
            {
                var visible = GetVisibleAsteroids(map, startingLocation);

                foreach (var asteroid in OrderByAngle(visible))
                {
                    yield return asteroid;

                    map.Remove(asteroid);

                    //map.Output(startingLocation, asteroid);
                }
            }
        }

        private static IEnumerable<(int x, int y)> OrderByAngle(Dictionary<double, (int x, int y)> possible)
        {
            const double halfPi = Math.PI / 2;
            const double negHalfPi = -Math.PI / 2;

            var all = possible.OrderBy(x => x.Key).ToArray();

            //ugh.
            var upperLeft = all.Where(x => x.Key <= halfPi && x.Key > 0).ToArray();
            var upperRight = all.Where(x => x.Key <= 0 && x.Key > negHalfPi).ToArray();
            var lowerRight = all.Where(x => x.Key <= negHalfPi && x.Key > -Math.PI).ToArray();
            var lowerLeft = all.Where(x => x.Key <= Math.PI && x.Key > halfPi).ToArray();

            return upperRight.OrderByDescending(x => x.Key).Concat(lowerRight.OrderByDescending(x => x.Key)).Concat(lowerLeft.OrderByDescending(x => x.Key)).Concat(upperLeft.OrderByDescending(x => x.Key)).Select(x => x.Value);
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

        public bool IsInBounds((int x, int y) location)
        {
            var bound = GetBounds();
            return location.x >= 0 && location.y >= 0 && location.x < bound.width && location.y < bound.height;
        }

        public void Remove((int x, int y) asteroid)
        {
            _map[asteroid.y][asteroid.x] = false;
        }

        public void Output((int x, int y) start, (int x, int y)? highlight = null)
        {
            Console.WriteLine();

            var bounds = GetBounds();
            for (int y = 0; y < bounds.height; y++)
            {
                for (int x = 0; x < bounds.width; x++)

                {
                    if ((x, y) == start)
                    {
                        Console.Write("@");
                    }
                    else
                    {
                        if ((x, y) == highlight)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                        }

                        Console.Write(IsAsteroid((x, y)) ? "#" : ".");

                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
