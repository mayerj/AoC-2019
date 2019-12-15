using System;
using System.Collections.Generic;
using System.Linq;

namespace Day15
{
    internal class AStar<TNode>
    {
        private readonly TNode _start;
        private readonly TNode _goal;
        private readonly Func<TNode, int> _h;
        private readonly Func<TNode, TNode, int> _d;
        private readonly Func<TNode, IEnumerable<TNode>> _neighbors;
        private readonly Dictionary<TNode, float> gScore = new Dictionary<TNode, float>();
        private readonly Dictionary<TNode, float> fScore = new Dictionary<TNode, float>();

        public AStar(TNode start, TNode goal, Func<TNode, int> h, Func<TNode, TNode, int> d, Func<TNode, IEnumerable<TNode>> neighbors)
        {
            _start = start;
            _goal = goal;
            _h = h;
            _d = d;
            _neighbors = neighbors;
        }

        float GetGScore(TNode node)
        {
            return GetScore(gScore, node);
        }

        float GetFScore(TNode node)
        {
            return GetScore(fScore, node);
        }
        static float GetScore(Dictionary<TNode, float> source, TNode node)
        {
            if (source.TryGetValue(node, out var val))
            {
                return val;
            }

            return float.PositiveInfinity;
        }

        internal bool TryRun(out List<TNode> result)
        {
            HashSet<TNode> opeSet = new HashSet<TNode>() { _start };
            Dictionary<TNode, TNode> cameFrom = new Dictionary<TNode, TNode>();
            gScore[_start] = 0;

            fScore[_start] = _h(_start);

            while (opeSet.Any())
            {
                var current = opeSet.OrderBy(GetFScore).First();

                if (current.Equals(_goal))
                {
                    return Reconstruct(cameFrom, current, out result);
                }

                opeSet.Remove(current);
                foreach (var neighbor in _neighbors(current))
                {
                    var tentativeGscore = GetGScore(current) + _d(current, neighbor);

                    if (tentativeGscore < GetGScore(neighbor))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGscore;
                        fScore[neighbor] = tentativeGscore + _h(neighbor);
                        opeSet.Add(neighbor);
                    }
                }
            }

            result = default;
            return false;
        }

        private bool Reconstruct(Dictionary<TNode, TNode> cameFrom, TNode current, out List<TNode> result)
        {
            result = new List<TNode>() { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];

                if (!current.Equals(_start))
                {
                    result.Add(current);
                }
            }

            result = result.AsEnumerable().Reverse().ToList();
            return true;
        }
    }
}