using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            //sVerify(111111, true);
            //sVerify(223450, false);
            //sVerify(123789, false);

            Verify(112233, true);
            Verify(123444, false);
            Verify(111122, true);

            Console.WriteLine(ScanRange(Range.low, Range.high).Count);
        }

        private static void Verify(int password, bool isValid)
        {
            Debug.Assert(IsValid(password) == isValid);
        }

        private static bool IsValid(int password)
        {
            List<int> digits = password.ToString().Select(x => int.Parse(x.ToString())).ToList();

            if (digits.Count != 6)
            {
                return false;
            }

            bool found = false;
            for (int i = 0; i <= 9; i++)
            {
                if (CheckSequence(digits, i))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }

            List<int> sorted = digits.OrderBy(x => x).ToList();
            if (!sorted.SequenceEqual(digits))
            {
                return false;
            }

            return true;
        }

        private static bool CheckSequence(List<int> digits, int digit)
        {
            int count = 1;
            for (int i = 1; i < digits.Count; i++)
            {
                if (digits[i - 1] != digit)
                {
                    if (count == 2)
                    {
                        return true;
                    }

                    count = 1;
                    continue;
                }

                if (digits[i - 1] == digits[i])
                {
                    count++;
                }
            }

            if (count == 2)
            {
                return true;
            }

            return false;
        }

        private static List<int> ScanRange(int low, int high)
        {
            List<int> list = new List<int>();
            for (int i = low; i <= high; i++)
            {
                if (IsValid(i))
                {
                    list.Add(i);
                }
            }

            return list;
        }

        private static (int low, int high) Range = (125730, 579381);
    }
}
