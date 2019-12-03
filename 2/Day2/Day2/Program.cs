﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day2
{
	class Program
	{
		static void Main(string[] args)
		{
			VerifyIntCode("1,0,0,0,99", "2,0,0,0,99");
			VerifyIntCode("2,3,0,3,99", "2,3,0,6,99");
			VerifyIntCode("2,4,4,5,99,0", "2,4,4,5,99,9801");
			VerifyIntCode("1,1,1,4,99,5,6,0,99", "30,1,1,4,2,5,6,0,99");

			var result = IntCode(Mangle(Parse(Input),(1, 12), (2, 2)));
			Print(result);

		}

		private static List<int> Mangle(List<int> list, params (int, int)[] patches)
		{
			foreach(var patch in patches)
			{
				list[patch.Item1] = patch.Item2;
			}

			return list;
		}

		private static void VerifyIntCode(string input, string expected)
		{
			var result = IntCode(Parse(input));

			Debug.Assert(expected == Stringify(result));
		}

		private static void Print(List<int> result)
		{
			Console.WriteLine(Stringify(result));
		}

		private static string Stringify(List<int> result)
		{
			return string.Join(",", result);
		}

		private static List<int> Parse(string input)
		{
			return input.Split(',').Select(x => int.Parse(x)).ToList();
		}

		private static List<int> IntCode(List<int> input)
		{
			return IntCode(0, input);
		}

		private static List<int> IntCode(int index, List<int> input)
		{
			bool run = true;
			while (run)
			{
				int opcode = input[index];

				switch (opcode)
				{
					case 1:
						{
							//add
							Math(input, index, (x, y) => x + y);
							break;
						}
					case 2:
						{
							//mult
							Math(input, index, (x, y) => x * y);
							break;
						}
					case 99:
						run = false;
						break;
					default:
						throw new InvalidOperationException();
				}

				index += 4;
			}

			return input;
		}

		private static void Math(List<int> input, int index, Func<int, int, int> func)
		{
			int index1 = input[index + 1];
			int index2 = input[index + 2];
			int destIndex = input[index + 3];

			input[destIndex] = func(input[index1], input[index2]);
		}

		const string Input = @"1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,13,1,19,1,19,10,23,2,10,23,27,1,27,6,31,1,13,31,35,1,13,35,39,1,39,10,43,2,43,13,47,1,47,9,51,2,51,13,55,1,5,55,59,2,59,9,63,1,13,63,67,2,13,67,71,1,71,5,75,2,75,13,79,1,79,6,83,1,83,5,87,2,87,6,91,1,5,91,95,1,95,13,99,2,99,6,103,1,5,103,107,1,107,9,111,2,6,111,115,1,5,115,119,1,119,2,123,1,6,123,0,99,2,14,0,0";
	}
}
