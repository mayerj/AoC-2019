using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Day16
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(StringRunFFT(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 4));

            Debug.Assert(StringRunFFT("80871224585914546619083218645595", 100) == "24176176");
            Debug.Assert(StringRunFFT("19617804207202209144916044189917", 100) == "73745418");
            Debug.Assert(StringRunFFT("69317163492948606335995924319873", 100) == "52432133");

            Console.WriteLine(StringRunFFT(Input, 100));

            Debug.Assert(StringRunFFT2("03036732577212944063491565474664", 100) == "84462026");
            Debug.Assert(StringRunFFT2("02935109699940807407585447034323", 100) == "78725270");
            Debug.Assert(StringRunFFT2("03081770884921959731165446850517", 100) == "53553731");

        }

        private static string StringRunFFT2(string input, int iterations)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0; i < 10000; i++)
            {
                sb.Append(input);
            }

            int[] convertedData = sb.ToString().Select(x => int.Parse(x.ToString())).ToArray();

            var value = RunFFT(convertedData, iterations);

            int offset = int.Parse(convertedData.Take(7).Select(x => x.ToString()).Aggregate((x, y) => x + y));

            return string.Join("", value.Skip(offset).Take(8));
        }

        private static string StringRunFFT(string input, int iterations)
        {
            return StringRunFFT(input.Select(x => int.Parse(x.ToString())).ToArray(), iterations);
        }

        private static string StringRunFFT(int[] input, int iterations)
        {
            return string.Join("", RunFFT(input, iterations).Take(8));
        }

        private static int[] RunFFT(int[] input, int iterations)
        {
            FFT fft = new FFT(input);

            for (int i = 0; i < iterations; i++)
            {
                var next = fft.RunPhase();

                //Console.WriteLine("Phase {0}:\t{1}", i + 1, string.Join(", ", next));

                fft = new FFT(next);
            }

            return fft.Input;
        }

        private static string Input = "59791911701697178620772166487621926539855976237879300869872931303532122404711706813176657053802481833015214226705058704017099411284046473395211022546662450403964137283487707691563442026697656820695854453826690487611172860358286255850668069507687936410599520475680695180527327076479119764897119494161366645257480353063266653306023935874821274026377407051958316291995144593624792755553923648392169597897222058613725620920233283869036501950753970029182181770358827133737490530431859833065926816798051237510954742209939957376506364926219879150524606056996572743773912030397695613203835011524677640044237824961662635530619875905369208905866913334027160178";
    }
}
