﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day14
{
    class Program
    {
        static void Main()
        {
            Verify(@"10 ORE => 10 A
1 ORE => 1 B
7 A, 1 B => 1 C
7 A, 1 C => 1 D
7 A, 1 D => 1 E
7 A, 1 E => 1 FUEL", (1, "FUEL"), (31, "ORE"));

            Verify(@"9 ORE => 2 A
8 ORE => 3 B
7 ORE => 5 C
3 A, 4 B => 1 AB
5 B, 7 C => 1 BC
4 C, 1 A => 1 CA
2 AB, 3 BC, 4 CA => 1 FUEL", (1, "FUEL"), (165, "ORE"));

            Verify(@"157 ORE => 5 NZVS
165 ORE => 6 DCFZ
44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL
12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ
179 ORE => 7 PSHF
177 ORE => 5 HKGWZ
7 DCFZ, 7 PSHF => 2 XJWVT
165 ORE => 2 GPVTF
3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT", (1, "FUEL"), (13312, "ORE"));

            var map = GetMap();
            RecursiveSolverProcessor processor = new RecursiveSolverProcessor(Parse(Input, map), map);
            Console.WriteLine(processor.GetRequiredOre(processor.FuelType, 1));

            VerifyTrillons(@"157 ORE => 5 NZVS
165 ORE => 6 DCFZ
44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL
12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ
179 ORE => 7 PSHF
177 ORE => 5 HKGWZ
7 DCFZ, 7 PSHF => 2 XJWVT
165 ORE => 2 GPVTF
3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT", 82892753);

            VerifyTrillons(@"2 VPVL, 7 FWMGM, 2 CXFTF, 11 MNCFX => 1 STKFG
17 NVRVD, 3 JNWZP => 8 VPVL
53 STKFG, 6 MNCFX, 46 VJHF, 81 HVMC, 68 CXFTF, 25 GNMV => 1 FUEL
22 VJHF, 37 MNCFX => 5 FWMGM
139 ORE => 4 NVRVD
144 ORE => 7 JNWZP
5 MNCFX, 7 RFSQX, 2 FWMGM, 2 VPVL, 19 CXFTF => 3 HVMC
5 VJHF, 7 MNCFX, 9 VPVL, 37 CXFTF => 6 GNMV
145 ORE => 6 MNCFX
1 NVRVD => 8 CXFTF
1 VJHF, 6 MNCFX => 4 RFSQX
176 ORE => 6 VJHF", 5586022);

            VerifyTrillons(@"171 ORE => 8 CNZTR
7 ZLQW, 3 BMBT, 9 XCVML, 26 XMNCP, 1 WPTQ, 2 MZWV, 1 RJRHP => 4 PLWSL
114 ORE => 4 BHXH
14 VRPVC => 6 BMBT
6 BHXH, 18 KTJDG, 12 WPTQ, 7 PLWSL, 31 FHTLT, 37 ZDVW => 1 FUEL
6 WPTQ, 2 BMBT, 8 ZLQW, 18 KTJDG, 1 XMNCP, 6 MZWV, 1 RJRHP => 6 FHTLT
15 XDBXC, 2 LTCX, 1 VRPVC => 6 ZLQW
13 WPTQ, 10 LTCX, 3 RJRHP, 14 XMNCP, 2 MZWV, 1 ZLQW => 1 ZDVW
5 BMBT => 4 WPTQ
189 ORE => 9 KTJDG
1 MZWV, 17 XDBXC, 3 XCVML => 2 XMNCP
12 VRPVC, 27 CNZTR => 2 XDBXC
15 KTJDG, 12 BHXH => 5 XCVML
3 BHXH, 2 VRPVC => 7 MZWV
121 ORE => 7 VRPVC
7 XCVML => 6 RJRHP
5 BHXH, 4 VRPVC => 5 LTCX", 460664);


            Console.WriteLine(GetTrillions(Input));
        }


        private static void VerifyTrillons(string input, long expected)
        {
            long min = GetTrillions(input);

            Debug.Assert(expected == min);
        }

        private static long GetTrillions(string input)
        {
            var map = GetMap();

            var givenOre = 1000000000000;

            var recipes = Parse(input, map);

            long min = givenOre / RecursiveSolverProcessor.GetOreForFuel(recipes, map);
            long max = givenOre;

            while (max > min + 1)
            {
                long check = (min + max) / 2;
                Console.WriteLine($"checking {check}");
                if (RecursiveSolverProcessor.GetOreForFuel(recipes, map, check) > givenOre)
                {
                    max = check;
                }
                else
                {
                    min = check;
                }
            }

            return min;
        }

        private static void Verify(string input, (int, string) desired, (long, string) required)
        {
            var map = GetMap();

            long requiredInput = RecursiveSolverProcessor.GetOreForFuel(Parse(input, map), map);

            Debug.Assert(requiredInput == required.Item1);
        }

        private static List<Recipe> Parse(string input, Func<string, int> map)
        {
            return input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(x => ParseReceipe(x, map)).ToList();
        }

        private static Func<string, int> GetMap()
        {
            Dictionary<string, int> cache = new Dictionary<string, int>();

            return x =>
            {
                if (cache.ContainsKey(x)) { return cache[x]; }

                if (cache.Count == 0)
                {
                    cache[x] = 0;
                    return 0;
                }

                cache[x] = cache.Values.Max() + 1;
                return cache[x];
            };
        }

        private static Recipe ParseReceipe(string data, Func<string, int> map)
        {
            string[] parts = data.Split(new[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);

            return new Recipe(parts[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), parts[1], map);
        }

        private static string Input = @"14 LQGXD, 6 TDLQ => 9 VGLV
1 WBQF, 2 JZKMJ => 5 TRSK
5 MGHZ, 5 ZLDQF => 8 HMVG
1 JWQH, 1 QFBC, 2 ZXVNM => 8 JFJZH
8 QTPX, 8 LDLWS => 6 NVZPS
2 QPWF, 1 PRWSM => 5 WHWF
1 QPWF, 8 LDLWS => 5 LZBQ
127 ORE => 1 MDPJB
4 WHWF => 4 KQHW
1 QBCKX, 3 TMTH => 4 WLFTZ
15 NPMPT, 4 TMTH => 6 QFBC
12 MDPJB => 9 PRWSM
5 QXHFH, 3 LCDVR, 24 MWFP, 1 MSFV, 1 BPDJL, 3 LQGXD, 2 DVGW => 2 KCPSH
6 FPZXN, 1 FQSK, 3 TMTH => 1 FBHW
25 PRWSM => 1 MGHZ
6 XWKXC, 5 TMTH, 1 PZTGX => 6 NTQZ
3 BPDJL, 3 DJWCL, 2 JZKMJ => 7 MWFP
5 JFJZH => 3 DJWCL
22 WRNJ, 12 TRSK => 5 TBGJC
3 HKWP => 1 PDRN
3 JWQH => 5 JZKMJ
4 WBQF => 2 BJNS
1 GNBQ => 9 FQSK
8 HMVG, 1 HQHD => 5 NJFNC
7 QBCKX, 1 FQSK => 9 NDCQ
3 XWKXC, 7 QFBC, 3 GPFRS, 2 LPQZ, 2 LQGXD, 20 LZKM, 1 QRTH => 8 TDTKT
1 QTPX => 3 LPQZ
2 QGVQC, 14 LDLWS => 1 NPMPT
1 QRTH, 7 BPDJL => 7 XWKXC
9 WLFTZ, 8 TDLQ => 6 GKPK
4 GNBQ => 3 QXHFH
3 TBGJC, 1 LPQZ => 3 DVGW
3 NDCQ, 1 KGZT => 7 FPZXN
36 WLFTZ, 1 KCPSH, 1 GKPK, 1 TDTKT, 3 CSPFK, 27 JZKMJ, 5 VGLV, 39 XWKXC => 1 FUEL
115 ORE => 7 QGVQC
21 NTQZ, 11 HQHD, 33 JFJZH, 3 NJFNC, 3 MSFV, 1 TRSK, 7 WRNJ => 9 CSPFK
3 DVGW => 4 TDLQ
5 FPZXN => 6 WRNJ
10 TSDLM, 17 XDKP, 3 PDRN => 2 HQHD
1 PCWS => 3 PZTGX
2 QXHFH => 5 JWQH
17 KQHW => 2 WBQF
139 ORE => 5 LDLWS
3 TSDLM => 9 KGZT
16 NPMPT => 3 QTPX
3 DVGW, 5 KVFMS, 3 WLFTZ => 6 GPFRS
1 PZTGX, 2 LCDVR, 13 TBGJC => 6 LZKM
5 ZXVNM, 2 QXHFH => 4 MSFV
4 XDKP, 7 FBHW, 2 PCWS => 3 LCDVR
3 TRSK => 7 KVFMS
10 LDLWS => 9 TMTH
2 TBGJC => 6 LQGXD
2 TRSK => 6 ZXVNM
4 KQHW, 1 NVZPS => 8 ZLDQF
2 LZBQ => 4 QBCKX
7 QBCKX => 6 TSDLM
152 ORE => 3 QPWF
2 TSDLM, 8 WHWF => 3 HKWP
19 FQSK => 8 QRTH
19 QTPX => 3 GNBQ
4 PDRN, 12 HKWP, 4 PCWS => 3 XDKP
6 LZBQ, 19 BJNS => 5 BPDJL
5 HKWP, 6 NVZPS => 3 PCWS";
    }
}
