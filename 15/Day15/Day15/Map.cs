using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Day15
{
    class Node
    {
        public Node(TileTypes type, Point location)
        {
            Type = type;
            Position = location;
        }

        public TileTypes Type { get; }
        public Point Position { get; }

        public Dictionary<Point, Node> Connections { get; } = new Dictionary<Point, Node>();
        public bool IsWall => Type == TileTypes.Wall;

        public bool IsUnknown => Type == TileTypes.Unknown;

        public void UpdateConnection(Node otherNode)
        {
            UpdateConnection(otherNode.Position, otherNode.Type, otherNode);
        }

        public void UpdateConnection(Point point, TileTypes tileTypes, Node other)
        {
            if (tileTypes == TileTypes.Wall)
            {
                Connections.Remove(point);
                return;
            }

            Connections[point] = other;
        }
    }

    internal class Map
    {
        private Dictionary<Point, Node> _nodes = new Dictionary<Point, Node>();

        public Map(Node location, Dictionary<Point, Node> nodes)
        {
            Location = location;
            _nodes = nodes;
        }

        public Dictionary<Point, Node> GetNodes()
        {
            return new Dictionary<Point, Node>(_nodes);
        }

        public Node Location { get; set; }

        internal Point? FindUnexploredTile(out Queue<Point> directions)
        {
            (Node position, Point unexplored) = FindUnexplored(Location);

            if (position == Location)
            {
                directions = new Queue<Point>();
                directions.Enqueue(unexplored);
                return unexplored;
            }
            else if (position != null)
            {
                directions = MapDirections(Location, position);
                return directions == null ? (Point?)null : unexplored;
            }

            directions = null;
            return null;
        }

        private Queue<Point> MapDirections(Node location, Node position)
        {
            AStar<Node> algo = new AStar<Node>(location, position, x => Taxicab(position, x), (x, y) => Taxicab(x, y), x => x.Connections.Values.Where(x => x != null && !x.IsWall));

            if (algo.TryRun(out List<Node> result))
            {
                return Convert(result);
            }

            return null;
        }

        private int Taxicab(Node position, Node x)
        {
            return Math.Abs(x.Position.X - position.Position.X) + Math.Abs(x.Position.Y - position.Position.Y);
        }

        private Queue<Point> Convert(List<Node> result)
        {
            CheckConsistency(result);

            return new Queue<Point>(result.Select(x => x.Position));
        }

        private void CheckConsistency(List<Node> result)
        {
            if (result.Count == 0)
            {
                return;
            }

            var last = result[0];
            for (int i = 1; i < result.Count; i++)
            {
                var current = result[i];

                Debug.Assert(Taxicab(last, current) == 1);

                last = current;
            }
        }

        private (Node position, Point unexplored) FindUnexplored(Node location)
        {
            HashSet<Node> seen = new HashSet<Node>();
            Stack<Node> nodes = new Stack<Node>();
            nodes.Push(location);

            while (nodes.Count != 0)
            {
                var look = nodes.Pop();

                if (!seen.Add(look))
                {
                    continue;
                }

                if (look.Connections.Any(x => x.Value.IsUnknown))
                {
                    return (look, look.Connections.First(x => x.Value.IsUnknown).Key);
                }
                else
                {
                    foreach (var conn in look.Connections.Values)
                    {
                        nodes.Push(conn);
                    }
                }
            }

            return (null, default);
        }

        internal RequestedDirections[] ComputePath(Point startingLocation, Point endingLocation)
        {
            var directions = MapDirections(_nodes[startingLocation], _nodes[endingLocation]).ToList();

            List<RequestedDirections> result = new List<RequestedDirections>();

            var location = startingLocation;
            for (int i = 0; i < directions.Count; i++)
            {
                var next = directions[i];

                if ((location + Vector.North) == next || (location - Vector.North) == next)
                {
                    result.Add(RequestedDirections.North);
                }
                else if ((location + Vector.South) == next || (location - Vector.South) == next)
                {
                    result.Add(RequestedDirections.South);
                }
                else if ((location + Vector.East) == next || (location - Vector.East) == next)
                {
                    result.Add(RequestedDirections.East);
                }
                else if ((location + Vector.West) == next || (location - Vector.West) == next)
                {
                    result.Add(RequestedDirections.West);
                }
                else
                {
                    throw new InvalidOperationException();
                }

                location = next;
            }

            return result.ToArray();
        }

        internal string ToString(Point? currentDestination, Queue<Point> directions, HashSet<Point> flooded = null)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Location {Location.Position}");
            sb.AppendLine($"Destination {currentDestination?.ToString()}");
            sb.AppendLine("---------------------------------------------------");

            int minNodeX = _nodes.Keys.Select(x => x.X).Min();
            int maxNodeX = _nodes.Keys.Select(x => x.X).Max();

            int minNodeY = _nodes.Keys.Select(x => x.Y).Min();
            int maxNodeY = _nodes.Keys.Select(x => x.Y).Max();

            for (int y = maxNodeY; y >= minNodeY; y--)
            {
                sb.Append($"{y:####}\t: ");
                for (int x = minNodeX - 1; x <= maxNodeX; x++)
                {
                    char symbol;

                    if (flooded != null && flooded.Contains(new Point(x, y)))
                    {
                        symbol = 'F';
                    }
                    else if (currentDestination.HasValue && currentDestination.Value == new Point(x, y))
                    {
                        symbol = '%';
                    }
                    else if (directions != null && directions.Contains(new Point(x, y)))
                    {
                        symbol = GetIndex(directions, new Point(x, y)).ToString()[0];
                    }
                    else if (!_nodes.TryGetValue(new Point(x, y), out Node node))
                    {
                        symbol = '?';
                    }
                    else if (node == Location)
                    {
                        symbol = 'D';
                    }
                    else
                    {
                        switch (node.Type)
                        {
                            case TileTypes.Free:
                                symbol = '.';
                                break;
                            case TileTypes.Wall:
                                symbol = '#';
                                break;
                            case TileTypes.Droid:
                                symbol = 'D';
                                break;
                            case TileTypes.Oxygen:
                                symbol = '@';
                                break;
                            default:
                                symbol = ' ';
                                break;
                        }
                    }

                    sb.Append(symbol);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private int GetIndex(Queue<Point> directions, Point point)
        {
            int i = 0;
            foreach (var p in directions)
            {
                if (p == point)
                {
                    return i;
                }
                i++;
            }

            return i;
        }
    }
}