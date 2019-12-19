using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Day17
{
    class Program
    {
        static void Main(string[] args)
        {
            VM vm = new VM(new Memory(Input.Split(new[] { ',' }).Select(x => long.Parse(x)).ToList()), null, Output);

            while (!vm.IsHalted)
            {
                vm.Run();
            }

            sb.RemoveAll(x => x.Count == 0);

            //Console.WriteLine(string.Join(Environment.NewLine, sb.Select(x => string.Join("", x))));

            string given = @"..#..........
..#..........
#######...###
#.#...#...#.#
#############
..#...#...#..
..#####...^..";

            List<(int x, int y)> intersections = FindIntersections(Parse(given));

            Debug.Assert(76 == GetAlignmentScore(intersections));


            intersections = FindIntersections(sb);

            Console.WriteLine(GetAlignmentScore(intersections));

            //VacuumRobot robot = new VacuumRobot(InputPart2, true);

            //robot.Run("A,A,B,A,C", "L,12", "R, 12", "L,12,R,12,L,8,L,8,R,12,L,8,L,8,L,10,R,8,R,12,L,10");
            //var result2 = robot.Run("A,B,C", "L,12,L,12,R,12,L,12", "", "");

            string expected = "L,12,L,12,R,12,L,12,L,12,R,12,L,8,L,8,R,12,L,8,L,8,L,10,R,8,R,12,L,10,R,8,R,12,L,12,L,12,R,12,L,8,L,8,R,12,L,8,L,8,L,10,R,8,R,12,L,12,L,12,R,12,L,8,L,8,R,12,L,8,L,8";
            var data = Compress(ParseToSymbols(expected)).ToList();

            var result = Minmax(expected, () => new VacuumRobot(InputPart2, false), data);

            Console.WriteLine(result);

        }

        private static List<(char direction, int length)> ParseToSymbols(string v)
        {
            string[] parts = v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            Debug.Assert(parts.Length % 2 == 0);

            List<(char direction, int length)> result = new List<(char direction, int length)>();
            for (int i = 1; i < parts.Length; i += 2)
            {
                result.Add((parts[i - 1][0], int.Parse(parts[i])));
            }

            return result;
        }

        private static IEnumerable<(List<(char direction, int length)> a, List<(char direction, int length)> b, List<(char direction, int length)> c)> Compress(List<(char direction, int length)> input)
        {
            Dictionary<List<(char direction, int length)>, int> dict = new Dictionary<List<(char direction, int length)>, int>(new SymbolEqualityComparer());

            foreach (var v in input)
            {
                if (!dict.ContainsKey(new List<(char direction, int length)>() { v }))
                {
                    dict.Add(new List<(char direction, int length)>() { v }, dict.Count == 0 ? 0 : dict.Values.Count());
                }
            }

            List<int> result = null;
            List<int> lastResult = null;
            do
            {
                lastResult = result;
                result = Compress(dict, input);

                foreach (int i in result)
                {
                    Console.WriteLine($"{i}: {string.Join(",", dict.First(x => x.Value == i).Key)}");
                }

                Console.WriteLine();
            } while (result == null || lastResult == null || lastResult.Count > result.Count);


            foreach (var abc in GetPossible(input, dict))
            {
                yield return (abc.a, abc.b, abc.c);
            }
        }

        private static IEnumerable<(List<(char direction, int length)> a, List<(char direction, int length)> b, List<(char direction, int length)> c)> GetPossible(List<(char direction, int length)> input, Dictionary<List<(char direction, int length)>, int> dict)
        {
            HashSet<(char direction, int length)> all = new HashSet<(char direction, int length)>(input);

            var result = GetAllCombinations(RemoveOverlength(dict.Keys).ToList(), 3);

            foreach (var a in result)
            {
                if (a.Count != 3)
                {
                    continue;
                }

                yield return (a[0], a[1], a[2]);
            }

            //foreach (var a in dict)
            //{
            //    foreach (var b in dict)
            //    {
            //        foreach (var c in dict)
            //        {
            //            if (a.Value == b.Value || b.Value == c.Value || c.Value == a.Value)
            //            {
            //                continue;
            //            }
            //
            //            HashSet<(char direction, int length)> current = new HashSet<(char direction, int length)>();
            //            current.UnionWith(a.Key);
            //            current.UnionWith(b.Key);
            //            current.UnionWith(c.Key);
            //
            //            if (!current.SetEquals(all))
            //            {
            //                continue;
            //            }
            //
            //            if (ToString(a.Key) != null && ToString(b.Key) != null && ToString(c.Key) != null)
            //            {
            //                yield return (a.Key, b.Key, c.Key);
            //            }
            //        }
            //    }
            //}
        }

        private static IEnumerable<List<(char direction, int length)>> RemoveOverlength(IEnumerable<List<(char direction, int length)>> keys)
        {
            return keys.Where(x => ToString(x) != null);
        }

        private static List<List<T>> GetAllCombinations<T>(List<T> list, int length)
        {
            var v = GetAllCombinations(list, length, new List<T>());

            return v;
        }

        private static List<List<T>> GetAllCombinations<T>(List<T> list1, int length, List<T> list2)
        {
            if (list2.Count == length)
            {
                return new List<List<T>> { list2 };
            }

            List<List<T>> combos = new List<List<T>>();
            for (int i = 0; i < list1.Count; i++)
            {
                var prev = new List<T>(list2);
                prev.Add(list1[i]);

                combos.AddRange(GetAllCombinations(list1.Skip(i + 1).ToList(), length, prev));
            }

            return combos;
        }

        private static List<int> Compress(Dictionary<List<(char direction, int length)>, int> dict, List<(char direction, int length)> input)
        {
            List<int> compressed = new List<int>();

            List<(char direction, int length)> output = new List<(char direction, int length)>();
            foreach (var c in input)
            {
                List<(char direction, int length)> current = output.Append(c).ToList();

                if (dict.TryGetValue(current, out var val))
                {
                    output = current;
                }
                else
                {
                    compressed.Add(dict[output]);

                    dict[current] = dict.Count;

                    output = new List<(char direction, int length)>() { c };
                }
            }

            if (output.Count != 0)
            {
                compressed.Add(dict[output]);
            }

            return compressed;
        }

        private static long Minmax(string expected, Func<VacuumRobot> getRobot, List<(List<(char direction, int length)> a, List<(char direction, int length)> b, List<(char direction, int length)> c)> abcs)
        {
            HashSet<string> failed = new HashSet<string>();

            long highest = 0;
            foreach (string main in GetMain())
            {
                Console.WriteLine(main);

                int i = 0;
                foreach (var abc in abcs)
                {
                    i++;

                    var a = ToString(abc.a);
                    var b = ToString(abc.b);
                    var c = ToString(abc.c);

                    if (a == "L,12,L,12,R,12" || b == "L,12,L,12,R,12" || c == "L,12,L,12,R,12")
                    {
                        if (a == "L,8,L,8,R,12,L,8,L,8" || b == "L,8,L,8,R,12,L,8,L,8" || c == "L,8,L,8,R,12,L,8,L,8")
                        {
                            if(a == "L,10,R,8,R,12" || b == "L,10,R,8,R,12" || c == "L,10,R,8,R,12")
                            {
                            }
                        }
                    }

                    var total = Expand(main, (a, b, c));

                    if (total != expected)
                    {
                        continue;
                    }

                    if (failed.Contains(total))
                    {
                        continue;
                    }

                    var bot = getRobot();

                    var result = bot.Run(main, a, b, c);

                    if (result.failed)
                    {
                        failed.Add(total);
                    }

                    if (!result.failed && result.score > highest)
                    {
                        Console.WriteLine($"{main}: ({ToString(abc.a)}),({ToString(abc.b)}),({ToString(abc.c)}): {result.score} {highest} {result.failed}");
                    }

                    highest = Math.Max(result.score, highest);
                }
            }

            return highest;
        }

        private static string ToString(List<(char direction, int length)> data)
        {
            var result = string.Join(",", data.Select(x => $"{x.direction},{x.length}"));

            if (result.Length > 20)
            {
                return null;
            }

            return result;
        }

        private static string Expand(string main, (string a, string b, string c) abc)
        {
            List<string> totals = new List<string>();
            foreach (string index in main.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string val = index switch
                {
                    "A" => abc.a,
                    "B" => abc.b,
                    "C" => abc.c,
                    _ => throw new ArgumentException()
                };

                if (val == string.Empty)
                {
                    continue;
                }

                totals.Add(val);
            }

            return string.Join(",", totals);
        }

        private static IEnumerable<(string a, string b, string c)> GetAbc(string main)
        {
            if (!main.Contains("A") || !main.Contains("B") || !main.Contains("C"))
            {
                yield break;
            }

            foreach (string[] a in GetA("A", main))
            {
                foreach (string[] b in GetA("B", main))
                {
                    foreach (string[] c in GetA("C", main))
                    {
                        yield return (string.Join(",", a), string.Join(",", b), string.Join(",", c));
                    }
                }
            }
        }

        private static IEnumerable<string[]> GetA(string needed, string main)
        {
            //yield return "";

            if (!main.Contains(needed))
            {
                yield break;
            }

            string[] possibleDirection = new[] { "L", "R" };
            string[] possibleLength = new[] { "10", "12", "8" };

            for (int i = 0; i <= 20; i++)
            {
                foreach (var s in GetA((possibleDirection, true), (possibleLength, false), i))
                {
                    if (s.Length < 2 || char.IsLetter(s.Last()[0]))
                    {
                        continue;
                    }

                    yield return s;
                }
            }
        }

        private static IEnumerable<string[]> GetA((string[] take, bool isLetter) input, (string[] take, bool isLetter) next, int maxLength)
        {
            if (maxLength <= 0)
            {
                yield break;
            }

            foreach (string i in input.take)
            {
                yield return new[] { i };

                foreach (var s in GetA(next, input, maxLength - i.Length))
                {
                    if (i.Length + 1 + s.Length + s.Sum(x => x.Length) <= maxLength)
                    {
                        yield return new[] { i }.Concat(s).ToArray();
                    }
                }
            }
        }

        private static IEnumerable<string> GetMain()
        {
            char[] possible = new[] { 'A', 'B', 'C' };

            yield return "A,A,B,C,C,A,B,C,A,B";
            yield return "B,B,C,A,A,B,C,A,B,C";
            yield return "C,C,A,B,B,C,A,B,C,A";
            yield return "A,A,C,B,B,A,C,B,A,C";
            yield break;

            for (int i = 0; i <= 20; i += 2)
            {
                foreach (var s in Generate(possible, i))
                {
                    yield return s;
                }
            }
        }

        private static IEnumerable<string> Generate(char[] possible, int maxLength)
        {
            if (maxLength == 0)
            {
                yield break;
            }

            foreach (char i in possible)
            {
                yield return $"{i}";

                foreach (var s in Generate(possible, maxLength - 2))
                {
                    yield return $"{i},{s}";
                }
            }
        }

        private static int GetAlignmentScore(List<(int x, int y)> intersections)
        {
            int sum = 0;
            foreach ((int x, int y) in intersections)
            {
                sum += (x * y);
            }

            return sum;
        }

        private static List<List<char>> Parse(string given)
        {
            List<List<char>> data = new List<List<char>>();

            foreach (string str in given.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                List<char> parts = new List<char>(str);
                data.Add(parts);
            }

            return data;
        }

        private static List<(int x, int y)> FindIntersections(List<List<char>> sb)
        {
            List<(int x, int y)> intersections = new List<(int x, int y)>();

            for (int y = 0; y < sb.Count; y++)
            {
                for (int x = 0; x < sb[y].Count; x++)
                {
                    if (sb[y][x] != '#')
                    {
                        continue;
                    }

                    IEnumerable<(int x, int y)> around = GetNeighbors(x, y, sb.Count, sb[y].Count);

                    if (around.All(poss => sb[poss.y][poss.x] == '#'))
                    {
                        intersections.Add((x, y));
                    }
                }
            }

            return intersections;
        }

        private static IEnumerable<(int x, int y)> GetNeighbors(int x, int y, int count1, int count2)
        {
            IEnumerable<(int x, int y)> GetPossible()
            {
                yield return (x, y - 1);
                yield return (x, y + 1);
                yield return (x - 1, y);
                yield return (x + 1, y);
            }

            foreach ((int x, int y) possible in GetPossible())
            {
                if (possible.x < 0 || possible.y < 0)
                {
                    continue;
                }

                if (possible.x >= count2 || possible.y >= count1)
                {
                    continue;
                }

                yield return (possible.x, possible.y);
            }
        }

        private static readonly List<List<char>> sb = new List<List<char>>() { new List<char>() };
        private static void Output(long obj)
        {
            char output = (char)obj;

            if (obj == 10)
            {
                sb.Add(new List<char>());
                return;
            }

            sb.Last().Add(output);
        }

        private static string Input = "1,330,331,332,109,4588,1101,1182,0,16,1102,1,1533,24,102,1,0,570,1006,570,36,102,1,571,0,1001,570,-1,570,1001,24,1,24,1106,0,18,1008,571,0,571,1001,16,1,16,1008,16,1533,570,1006,570,14,21101,0,58,0,1106,0,786,1006,332,62,99,21102,333,1,1,21101,73,0,0,1106,0,579,1102,0,1,572,1102,1,0,573,3,574,101,1,573,573,1007,574,65,570,1005,570,151,107,67,574,570,1005,570,151,1001,574,-64,574,1002,574,-1,574,1001,572,1,572,1007,572,11,570,1006,570,165,101,1182,572,127,1002,574,1,0,3,574,101,1,573,573,1008,574,10,570,1005,570,189,1008,574,44,570,1006,570,158,1106,0,81,21101,340,0,1,1106,0,177,21101,477,0,1,1105,1,177,21102,1,514,1,21102,176,1,0,1106,0,579,99,21101,0,184,0,1106,0,579,4,574,104,10,99,1007,573,22,570,1006,570,165,1002,572,1,1182,21102,1,375,1,21101,0,211,0,1106,0,579,21101,1182,11,1,21101,0,222,0,1105,1,979,21101,0,388,1,21101,0,233,0,1105,1,579,21101,1182,22,1,21102,244,1,0,1106,0,979,21102,401,1,1,21101,255,0,0,1106,0,579,21101,1182,33,1,21102,266,1,0,1105,1,979,21102,1,414,1,21102,277,1,0,1105,1,579,3,575,1008,575,89,570,1008,575,121,575,1,575,570,575,3,574,1008,574,10,570,1006,570,291,104,10,21101,0,1182,1,21101,313,0,0,1106,0,622,1005,575,327,1101,0,1,575,21101,0,327,0,1106,0,786,4,438,99,0,1,1,6,77,97,105,110,58,10,33,10,69,120,112,101,99,116,101,100,32,102,117,110,99,116,105,111,110,32,110,97,109,101,32,98,117,116,32,103,111,116,58,32,0,12,70,117,110,99,116,105,111,110,32,65,58,10,12,70,117,110,99,116,105,111,110,32,66,58,10,12,70,117,110,99,116,105,111,110,32,67,58,10,23,67,111,110,116,105,110,117,111,117,115,32,118,105,100,101,111,32,102,101,101,100,63,10,0,37,10,69,120,112,101,99,116,101,100,32,82,44,32,76,44,32,111,114,32,100,105,115,116,97,110,99,101,32,98,117,116,32,103,111,116,58,32,36,10,69,120,112,101,99,116,101,100,32,99,111,109,109,97,32,111,114,32,110,101,119,108,105,110,101,32,98,117,116,32,103,111,116,58,32,43,10,68,101,102,105,110,105,116,105,111,110,115,32,109,97,121,32,98,101,32,97,116,32,109,111,115,116,32,50,48,32,99,104,97,114,97,99,116,101,114,115,33,10,94,62,118,60,0,1,0,-1,-1,0,1,0,0,0,0,0,0,1,38,28,0,109,4,2102,1,-3,586,21001,0,0,-1,22101,1,-3,-3,21101,0,0,-2,2208,-2,-1,570,1005,570,617,2201,-3,-2,609,4,0,21201,-2,1,-2,1105,1,597,109,-4,2106,0,0,109,5,2102,1,-4,629,21001,0,0,-2,22101,1,-4,-4,21102,0,1,-3,2208,-3,-2,570,1005,570,781,2201,-4,-3,652,21002,0,1,-1,1208,-1,-4,570,1005,570,709,1208,-1,-5,570,1005,570,734,1207,-1,0,570,1005,570,759,1206,-1,774,1001,578,562,684,1,0,576,576,1001,578,566,692,1,0,577,577,21101,702,0,0,1106,0,786,21201,-1,-1,-1,1105,1,676,1001,578,1,578,1008,578,4,570,1006,570,724,1001,578,-4,578,21102,731,1,0,1105,1,786,1106,0,774,1001,578,-1,578,1008,578,-1,570,1006,570,749,1001,578,4,578,21101,756,0,0,1106,0,786,1106,0,774,21202,-1,-11,1,22101,1182,1,1,21102,774,1,0,1105,1,622,21201,-3,1,-3,1106,0,640,109,-5,2106,0,0,109,7,1005,575,802,20101,0,576,-6,21002,577,1,-5,1105,1,814,21101,0,0,-1,21101,0,0,-5,21102,0,1,-6,20208,-6,576,-2,208,-5,577,570,22002,570,-2,-2,21202,-5,47,-3,22201,-6,-3,-3,22101,1533,-3,-3,2101,0,-3,843,1005,0,863,21202,-2,42,-4,22101,46,-4,-4,1206,-2,924,21102,1,1,-1,1105,1,924,1205,-2,873,21102,35,1,-4,1106,0,924,2101,0,-3,878,1008,0,1,570,1006,570,916,1001,374,1,374,1201,-3,0,895,1101,2,0,0,2102,1,-3,902,1001,438,0,438,2202,-6,-5,570,1,570,374,570,1,570,438,438,1001,578,558,922,20101,0,0,-4,1006,575,959,204,-4,22101,1,-6,-6,1208,-6,47,570,1006,570,814,104,10,22101,1,-5,-5,1208,-5,65,570,1006,570,810,104,10,1206,-1,974,99,1206,-1,974,1102,1,1,575,21102,973,1,0,1106,0,786,99,109,-7,2105,1,0,109,6,21101,0,0,-4,21102,0,1,-3,203,-2,22101,1,-3,-3,21208,-2,82,-1,1205,-1,1030,21208,-2,76,-1,1205,-1,1037,21207,-2,48,-1,1205,-1,1124,22107,57,-2,-1,1205,-1,1124,21201,-2,-48,-2,1105,1,1041,21102,-4,1,-2,1106,0,1041,21102,-5,1,-2,21201,-4,1,-4,21207,-4,11,-1,1206,-1,1138,2201,-5,-4,1059,2101,0,-2,0,203,-2,22101,1,-3,-3,21207,-2,48,-1,1205,-1,1107,22107,57,-2,-1,1205,-1,1107,21201,-2,-48,-2,2201,-5,-4,1090,20102,10,0,-1,22201,-2,-1,-2,2201,-5,-4,1103,1202,-2,1,0,1105,1,1060,21208,-2,10,-1,1205,-1,1162,21208,-2,44,-1,1206,-1,1131,1105,1,989,21101,0,439,1,1105,1,1150,21101,0,477,1,1106,0,1150,21102,1,514,1,21102,1149,1,0,1105,1,579,99,21102,1157,1,0,1105,1,579,204,-2,104,10,99,21207,-3,22,-1,1206,-1,1138,2102,1,-5,1176,2101,0,-4,0,109,-6,2105,1,0,24,9,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,46,1,46,1,46,1,46,9,46,1,18,9,19,1,18,1,7,1,19,1,18,1,7,1,19,1,18,1,7,1,19,1,18,1,7,1,19,1,18,1,7,1,19,1,14,9,3,1,7,13,14,1,3,1,3,1,3,1,7,1,18,13,3,1,3,1,7,1,18,1,7,1,7,1,3,1,7,1,18,1,7,1,7,1,3,1,7,1,18,1,7,1,7,1,3,1,7,1,18,1,7,1,7,1,3,13,14,1,7,1,7,1,11,1,3,1,14,1,7,1,7,1,9,13,8,1,7,1,7,1,9,1,1,1,3,1,14,9,7,1,9,1,1,1,3,1,30,1,9,1,1,1,3,1,30,13,3,1,40,1,5,1,40,1,5,1,40,1,5,1,40,1,5,1,40,1,5,1,34,13,34,1,5,1,34,13,34,1,5,1,40,1,5,1,40,1,5,1,40,1,5,1,40,1,5,1,40,1,5,11,30,1,15,1,30,1,15,1,7,9,14,1,15,1,7,1,7,1,14,1,15,1,7,1,7,1,14,1,15,1,7,1,7,1,14,13,3,1,7,1,7,1,26,1,3,1,7,1,7,1,26,1,3,1,7,1,7,1,26,1,3,1,7,1,7,1,26,1,3,1,3,13,26,1,3,1,3,1,3,1,34,1,3,9,34,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,9,12";
        private static string InputPart2 = "2,330,331,332,109,4588,1101,1182,0,16,1102,1,1533,24,102,1,0,570,1006,570,36,102,1,571,0,1001,570,-1,570,1001,24,1,24,1106,0,18,1008,571,0,571,1001,16,1,16,1008,16,1533,570,1006,570,14,21101,0,58,0,1106,0,786,1006,332,62,99,21102,333,1,1,21101,73,0,0,1106,0,579,1102,0,1,572,1102,1,0,573,3,574,101,1,573,573,1007,574,65,570,1005,570,151,107,67,574,570,1005,570,151,1001,574,-64,574,1002,574,-1,574,1001,572,1,572,1007,572,11,570,1006,570,165,101,1182,572,127,1002,574,1,0,3,574,101,1,573,573,1008,574,10,570,1005,570,189,1008,574,44,570,1006,570,158,1106,0,81,21101,340,0,1,1106,0,177,21101,477,0,1,1105,1,177,21102,1,514,1,21102,176,1,0,1106,0,579,99,21101,0,184,0,1106,0,579,4,574,104,10,99,1007,573,22,570,1006,570,165,1002,572,1,1182,21102,1,375,1,21101,0,211,0,1106,0,579,21101,1182,11,1,21101,0,222,0,1105,1,979,21101,0,388,1,21101,0,233,0,1105,1,579,21101,1182,22,1,21102,244,1,0,1106,0,979,21102,401,1,1,21101,255,0,0,1106,0,579,21101,1182,33,1,21102,266,1,0,1105,1,979,21102,1,414,1,21102,277,1,0,1105,1,579,3,575,1008,575,89,570,1008,575,121,575,1,575,570,575,3,574,1008,574,10,570,1006,570,291,104,10,21101,0,1182,1,21101,313,0,0,1106,0,622,1005,575,327,1101,0,1,575,21101,0,327,0,1106,0,786,4,438,99,0,1,1,6,77,97,105,110,58,10,33,10,69,120,112,101,99,116,101,100,32,102,117,110,99,116,105,111,110,32,110,97,109,101,32,98,117,116,32,103,111,116,58,32,0,12,70,117,110,99,116,105,111,110,32,65,58,10,12,70,117,110,99,116,105,111,110,32,66,58,10,12,70,117,110,99,116,105,111,110,32,67,58,10,23,67,111,110,116,105,110,117,111,117,115,32,118,105,100,101,111,32,102,101,101,100,63,10,0,37,10,69,120,112,101,99,116,101,100,32,82,44,32,76,44,32,111,114,32,100,105,115,116,97,110,99,101,32,98,117,116,32,103,111,116,58,32,36,10,69,120,112,101,99,116,101,100,32,99,111,109,109,97,32,111,114,32,110,101,119,108,105,110,101,32,98,117,116,32,103,111,116,58,32,43,10,68,101,102,105,110,105,116,105,111,110,115,32,109,97,121,32,98,101,32,97,116,32,109,111,115,116,32,50,48,32,99,104,97,114,97,99,116,101,114,115,33,10,94,62,118,60,0,1,0,-1,-1,0,1,0,0,0,0,0,0,1,38,28,0,109,4,2102,1,-3,586,21001,0,0,-1,22101,1,-3,-3,21101,0,0,-2,2208,-2,-1,570,1005,570,617,2201,-3,-2,609,4,0,21201,-2,1,-2,1105,1,597,109,-4,2106,0,0,109,5,2102,1,-4,629,21001,0,0,-2,22101,1,-4,-4,21102,0,1,-3,2208,-3,-2,570,1005,570,781,2201,-4,-3,652,21002,0,1,-1,1208,-1,-4,570,1005,570,709,1208,-1,-5,570,1005,570,734,1207,-1,0,570,1005,570,759,1206,-1,774,1001,578,562,684,1,0,576,576,1001,578,566,692,1,0,577,577,21101,702,0,0,1106,0,786,21201,-1,-1,-1,1105,1,676,1001,578,1,578,1008,578,4,570,1006,570,724,1001,578,-4,578,21102,731,1,0,1105,1,786,1106,0,774,1001,578,-1,578,1008,578,-1,570,1006,570,749,1001,578,4,578,21101,756,0,0,1106,0,786,1106,0,774,21202,-1,-11,1,22101,1182,1,1,21102,774,1,0,1105,1,622,21201,-3,1,-3,1106,0,640,109,-5,2106,0,0,109,7,1005,575,802,20101,0,576,-6,21002,577,1,-5,1105,1,814,21101,0,0,-1,21101,0,0,-5,21102,0,1,-6,20208,-6,576,-2,208,-5,577,570,22002,570,-2,-2,21202,-5,47,-3,22201,-6,-3,-3,22101,1533,-3,-3,2101,0,-3,843,1005,0,863,21202,-2,42,-4,22101,46,-4,-4,1206,-2,924,21102,1,1,-1,1105,1,924,1205,-2,873,21102,35,1,-4,1106,0,924,2101,0,-3,878,1008,0,1,570,1006,570,916,1001,374,1,374,1201,-3,0,895,1101,2,0,0,2102,1,-3,902,1001,438,0,438,2202,-6,-5,570,1,570,374,570,1,570,438,438,1001,578,558,922,20101,0,0,-4,1006,575,959,204,-4,22101,1,-6,-6,1208,-6,47,570,1006,570,814,104,10,22101,1,-5,-5,1208,-5,65,570,1006,570,810,104,10,1206,-1,974,99,1206,-1,974,1102,1,1,575,21102,973,1,0,1106,0,786,99,109,-7,2105,1,0,109,6,21101,0,0,-4,21102,0,1,-3,203,-2,22101,1,-3,-3,21208,-2,82,-1,1205,-1,1030,21208,-2,76,-1,1205,-1,1037,21207,-2,48,-1,1205,-1,1124,22107,57,-2,-1,1205,-1,1124,21201,-2,-48,-2,1105,1,1041,21102,-4,1,-2,1106,0,1041,21102,-5,1,-2,21201,-4,1,-4,21207,-4,11,-1,1206,-1,1138,2201,-5,-4,1059,2101,0,-2,0,203,-2,22101,1,-3,-3,21207,-2,48,-1,1205,-1,1107,22107,57,-2,-1,1205,-1,1107,21201,-2,-48,-2,2201,-5,-4,1090,20102,10,0,-1,22201,-2,-1,-2,2201,-5,-4,1103,1202,-2,1,0,1105,1,1060,21208,-2,10,-1,1205,-1,1162,21208,-2,44,-1,1206,-1,1131,1105,1,989,21101,0,439,1,1105,1,1150,21101,0,477,1,1106,0,1150,21102,1,514,1,21102,1149,1,0,1105,1,579,99,21102,1157,1,0,1105,1,579,204,-2,104,10,99,21207,-3,22,-1,1206,-1,1138,2102,1,-5,1176,2101,0,-4,0,109,-6,2105,1,0,24,9,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,46,1,46,1,46,1,46,9,46,1,18,9,19,1,18,1,7,1,19,1,18,1,7,1,19,1,18,1,7,1,19,1,18,1,7,1,19,1,18,1,7,1,19,1,14,9,3,1,7,13,14,1,3,1,3,1,3,1,7,1,18,13,3,1,3,1,7,1,18,1,7,1,7,1,3,1,7,1,18,1,7,1,7,1,3,1,7,1,18,1,7,1,7,1,3,1,7,1,18,1,7,1,7,1,3,13,14,1,7,1,7,1,11,1,3,1,14,1,7,1,7,1,9,13,8,1,7,1,7,1,9,1,1,1,3,1,14,9,7,1,9,1,1,1,3,1,30,1,9,1,1,1,3,1,30,13,3,1,40,1,5,1,40,1,5,1,40,1,5,1,40,1,5,1,40,1,5,1,34,13,34,1,5,1,34,13,34,1,5,1,40,1,5,1,40,1,5,1,40,1,5,1,40,1,5,1,40,1,5,11,30,1,15,1,30,1,15,1,7,9,14,1,15,1,7,1,7,1,14,1,15,1,7,1,7,1,14,1,15,1,7,1,7,1,14,13,3,1,7,1,7,1,26,1,3,1,7,1,7,1,26,1,3,1,7,1,7,1,26,1,3,1,7,1,7,1,26,1,3,1,3,13,26,1,3,1,3,1,3,1,34,1,3,9,34,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,1,7,1,38,9,12";
    }

    internal class SymbolEqualityComparer : EqualityComparer<List<(char direction, int length)>>
    {
        public override bool Equals([AllowNull] List<(char direction, int length)> x, [AllowNull] List<(char direction, int length)> y)
        {
            return x.SequenceEqual(y);
        }

        public override int GetHashCode([DisallowNull] List<(char direction, int length)> obj)
        {
            HashCode hc = new HashCode();

            foreach (var v in obj)
            {
                hc.Add(v);
            }

            return hc.ToHashCode();
        }
    }
}
