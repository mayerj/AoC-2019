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
...##", @"
.7..7
.....
67775
....7
...87");
        }

        private static void Check(string input, (int x, int y) expected)
        {
            Map map = Parse(input);

            var optimal = FindOptimal(map);

            Debug.Assert(optimal.OrderByDescending(x => x.Value).First().Key == expected);
        }

        private static void Check(string input, string expected)
        {
            Map map = Parse(input);

            var optimal = FindOptimal(map);

            var expectedData = ParseExpected(expected);

            foreach(var data in optimal)
            {
                Debug.Assert(data.Value == expectedData[data.Key.x][data.Key.y]);
            }
        }

        private static Dictionary<(int x, int y), int> FindOptimal(Map map)
        {
            Dictionary<(int x, int y), int> locations = new Dictionary<(int x, int y), int>();
            foreach (var location in GetAsteroids(map))
            {
                locations[location] = GetVisibleAsteroids(map, location);
            }

            return locations;
        }

        private static int GetVisibleAsteroids(Map map, (int x, int y) location)
        {
            int visibleAsteroids = 0;
            foreach (var asteroid in GetAsteroids(map))
            {
                if (IsVisible(asteroid, map, location))
                {
                    visibleAsteroids++;
                }
            }

            return visibleAsteroids;
        }

        private static bool IsVisible((int x, int y) asteroid, Map map, (int x, int y) location)
        {
            var directions = new (int x, int y)[] { (0, -1), (0, 1), (1, -1), (1, 0), (1, 1), (-1, 1), (-1, 0), (-1, -1) };

            foreach (var delta in directions)
            {
                var los = location;
                do
                {
                    los = (los.x + delta.x, los.y + delta.y);

                    if (map.IsAsteroid(los))
                    {
                        if (los == asteroid)
                        {
                            return true;
                        }
                        break;
                    }

                } while (map.IsInBounds(los));
            }

            return false;
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

        public bool IsAsteroid((int x, int y) location) => IsInBounds(location) && _map[location.x][location.y];

        internal bool IsInBounds((int x, int y) location)
        {
            var bound = GetBounds();
            return location.x >= 0 && location.y >= 0 && location.x < bound.width && location.y < bound.height;
        }
    }
}
