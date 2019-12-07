using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day7
{
    class Program
    {
        static void Main(string[] args)
        {
            VerifyThrusterSettings("3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0", 43210, "4,3,2,1,0");
            VerifyThrusterSettings("3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0", 54321, "0,1,2,3,4");
            VerifyThrusterSettings("3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0", 65210, "1,0,4,3,2");

            Console.WriteLine(FindOptimalSettings(Input));
        }

        private static string FindOptimalSettings(string input)
        {
            HashSet<int> settings = new HashSet<int> { 0, 1, 2, 3, 4 };

            return FindOptimalSettings(new List<int>(), settings, input).ToString();
        }

        private static int FindOptimalSettings(List<int> selectedSettings, HashSet<int> settings, string input)
        {
            int currentMax = 0;
            foreach (var setting in settings)
            {
                if (selectedSettings.Contains(setting))
                {
                    continue;
                }

                List<int> currentSelected = new List<int>(selectedSettings);
                currentSelected.Add(setting);

                if(currentSelected.Count == settings.Count)
                {
                    currentMax = Math.Max(currentMax, GetThrusterSettings(input, currentSelected));
                }
                else
                {
                    currentMax = Math.Max(currentMax, FindOptimalSettings(currentSelected, settings, input));
                }
            }

            return currentMax;
        }

        private static int GetThrusterSettings(string input, List<int> selectedSettings)
        {
            Console.WriteLine($"Checking {string.Join(",", selectedSettings)}");

            Thruster[] thrusters = selectedSettings.Select(x => new Thruster(input, x)).ToArray();

            int result = 0;
            for (int i = 0; i < thrusters.Length; i++)
            {
                result = thrusters[i].Run(result);
            }

            return result;
        }

        private static void VerifyThrusterSettings(string program, int expectedOutput, string expectedSequence)
        {
            var result = GetThrusterSettings(program, expectedSequence.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList());

            Debug.Assert(result == expectedOutput);
        }

        private static readonly string Input = "3,8,1001,8,10,8,105,1,0,0,21,42,67,88,101,114,195,276,357,438,99999,3,9,101,3,9,9,1002,9,4,9,1001,9,5,9,102,4,9,9,4,9,99,3,9,1001,9,3,9,1002,9,2,9,101,2,9,9,102,2,9,9,1001,9,5,9,4,9,99,3,9,102,4,9,9,1001,9,3,9,102,4,9,9,101,4,9,9,4,9,99,3,9,101,2,9,9,1002,9,3,9,4,9,99,3,9,101,4,9,9,1002,9,5,9,4,9,99,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,99,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,99,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,101,2,9,9,4,9,99";
    }
}
