using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Day1
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(Compute(new[] { 14 }) == 2);
			Console.WriteLine(Compute(new[] { 1969 }) == 966);
			Console.WriteLine(Compute(new[] { 100756 }) == 50346);

			Console.WriteLine(Compute(ParseData(data)));
		}

		static int Compute(IEnumerable<int> moduleMasses)
		{
			int totalFuel = 0;
			foreach (int moduleMass in moduleMasses)
			{
				int neededFuel = ComputeFuel(moduleMass);

				int fuelFuel = neededFuel;
				while (fuelFuel > 0)
				{
					fuelFuel = ComputeFuel(fuelFuel);

					if (fuelFuel > 0)
					{
						neededFuel += fuelFuel;
					}
				}

				totalFuel += neededFuel;
			}

			return totalFuel;
		}

		private static int ComputeFuel(int moduleMass)
		{
			int m = (moduleMass / 3);

			int fuelNeeded = m - 2;

			return fuelNeeded;
		}

		private static IEnumerable<int> ParseData(string data)
		{
			foreach (string str in data.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries))
			{
				yield return int.Parse(str);
			}
		}

		private const string data = @"73617
104372
131825
85022
105514
78478
87420
118553
97680
89479
146989
79746
108085
117895
143811
102509
102382
92975
72978
94208
130521
83042
138605
107566
63374
71176
129487
118408
115425
63967
98282
121829
92834
61084
70122
87221
132773
141347
133225
81199
94994
60881
110074
63499
143107
76618
86818
135394
106908
96085
99801
112903
51751
56002
70924
62180
133025
68025
122660
64898
77339
62109
133891
134460
84224
54836
59748
125540
67796
71845
92899
130103
74612
136820
96212
132002
97405
82629
63717
62805
112693
147810
139827
116220
69711
50236
137833
103743
147456
112098
84867
75615
132738
81072
89444
58443
94465
112494
82127
132533";
	}
}