using System;
using System.Collections.Generic;
using System.Linq;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            RepairBot bot = new RepairBot(new AutonomousBrain());

            var oxy = bot.FindOxygen();

            Console.WriteLine(bot.ComputeOptimalPath(new Point(0, 0)).Length);

            Map map = bot.GetMap();
            var nodes = map.GetNodes();

            var floodableNodes = new HashSet<Node>(nodes.Values.Where(x => x.Type == TileTypes.Droid || x.Type == TileTypes.Oxygen || x.Type == TileTypes.Free));

            HashSet<Node> floodedNodes = new HashSet<Node>() { nodes[oxy] };
            int minutes = 0;
            while (!floodedNodes.SetEquals(floodableNodes))
            {
                foreach (var flooded in new HashSet<Node>(floodedNodes))
                {
                    FloodNeighbors(floodedNodes, floodableNodes, flooded);
                }

                //Console.Clear();
                //Console.WriteLine(map.ToString(null, null, new HashSet<Point>(floodedNodes.Select(x=>x.Position))));
                //Console.ReadLine();
                minutes++;
            }

            Console.WriteLine($"Flooded in {minutes} minutes");
        }

        private static void FloodNeighbors(HashSet<Node> floodedNodes, HashSet<Node> floodable, Node flooded)
        {
            var neighbors = flooded.Connections.Values.Intersect(floodable).ToArray();

            floodedNodes.UnionWith(neighbors);
        }
    }

    class AutonomousBrain : IBrain
    {
        private readonly Dictionary<Point, Node> _nodes = new Dictionary<Point, Node>();
        private readonly Node _start = new Node(TileTypes.Free, new Point(0, 0));
        private readonly Map _map;

        private readonly RemoteControlledBrain _remote = new RemoteControlledBrain();
        private Point? _currentDestination = null;
        private Queue<Point> _directions = null;
        private Node _oxygen;

        public AutonomousBrain()
        {
            _start.UpdateConnection(_start.Position + Vector.North, TileTypes.Unknown, new Node(TileTypes.Unknown, _start.Position + Vector.North));
            _start.UpdateConnection(_start.Position + Vector.South, TileTypes.Unknown, new Node(TileTypes.Unknown, _start.Position + Vector.South));
            _start.UpdateConnection(_start.Position + Vector.East, TileTypes.Unknown, new Node(TileTypes.Unknown, _start.Position + Vector.East));
            _start.UpdateConnection(_start.Position + Vector.West, TileTypes.Unknown, new Node(TileTypes.Unknown, _start.Position + Vector.West));
            _nodes.Add(new Point(0, 0), _start);
            _map = new Map(_start, _nodes);
        }
        public void Map()
        {
            Console.Clear();
            Console.WriteLine($"Destination {_currentDestination?.ToString()}");
            Console.WriteLine(_map.ToString(_currentDestination, _directions));
        }

        public Map GetMap()
        {
            return _map;
        }

        public RequestedDirections? GetDirection(Point location, Dictionary<Point, TileTypes> tiles)
        {
            _map.Location = _nodes[location];

            if (_currentDestination != null && _currentDestination == location || _directions?.Count == 0)
            {
                _currentDestination = null;
            }

            if (_currentDestination == null)
            {
                //Map();

                _currentDestination = _map.FindUnexploredTile(out _directions);
            }

            if (_directions != null && _directions.Count > 0)
            {
                var next = _directions.Dequeue();

                var result = ConvertToMovement(location, next);
                return result;
            }

            return null; // _remote.GetDirection(location, tiles);
        }

        public void Report(Point point, TileTypes type)
        {
            if (_nodes.ContainsKey(point))
            {
                return;
            }

            Node node;
            if (type == TileTypes.Oxygen)
            {
                _oxygen = node = new Node(TileTypes.Oxygen, point);
            }
            else
            {
                node = new Node(type, point);
            }

            _nodes[point] = node;

            UpdateConnectivity(_nodes[point]);
        }

        private void UpdateConnectivity(Node node)
        {
            foreach (Point point in new[] { node.Position + Vector.North, node.Position + Vector.South, node.Position + Vector.East, node.Position + Vector.West })
            {
                if (_nodes.TryGetValue(point, out var otherNode))
                {
                    otherNode.UpdateConnection(node);
                    node.UpdateConnection(otherNode);
                }
                else
                {
                    node.UpdateConnection(point, TileTypes.Unknown, new Node(TileTypes.Unknown, point));
                }
            }
        }


        public RequestedDirections[] GetPath(Point startingLocation, Point endingLocation)
        {
            return _map.ComputePath(startingLocation, endingLocation);
        }

        private RequestedDirections? ConvertToMovement(Point location, Point next)
        {
            if ((location + Vector.North) == next)
            {
                return RequestedDirections.North;
            }

            if ((location + Vector.South) == next)
            {
                return RequestedDirections.South;
            }

            if ((location + Vector.East) == next)
            {
                return RequestedDirections.East;
            }

            if ((location + Vector.West) == next)
            {
                return RequestedDirections.West;
            }

            return null;
        }
    }

    class RemoteControlledBrain : IBrain
    {
        public void Report(Point point, TileTypes type)
        {
        }



        public void Map() { }
        public RequestedDirections? GetDirection(Point location, Dictionary<Point, TileTypes> tiles)
        {
            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        return RequestedDirections.North;
                    case ConsoleKey.DownArrow:
                        return RequestedDirections.South;
                    case ConsoleKey.LeftArrow:
                        return RequestedDirections.East;
                    case ConsoleKey.RightArrow:
                        return RequestedDirections.West;
                }
            }
        }

        public RequestedDirections[] GetPath(Point startingLocation, Point endingLocation)
        {
            throw new NotImplementedException();
        }

        public Map GetMap()
        {
            return null;
        }
    }
}
