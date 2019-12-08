﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day8
{
    class Program
    {
        static void Main(string[] args)
        {
            VerifyImage(@"123456789012", 3, 2, new[] { "123", "456" }, new[] { "789", "012" });

            Console.WriteLine(Find(Input, 25, 6));
        }

        private static int Find(string input, int width, int height)
        {
            Image image = Parse(input, width, height);

            Layer fewest = GetLayer(image, x => x == 0);

            return GetCount(fewest, 1) * GetCount(fewest, 2);
        }

        private static int GetCount(Layer layer, int value)
        {
            return layer.Data.SelectMany(x => x).Count(x => x == value);
        }

        private static Layer GetLayer(Image image, Func<int, bool> p)
        {
            Dictionary<Layer, int> data = new Dictionary<Layer, int>();
            foreach (Layer l in image.Layers)
            {
                data[l] = l.Data.SelectMany(x => x).Where(p).Count();
            }

            return data.OrderBy(x => x.Value).First().Key;
        }

        private static void VerifyImage(string input, int width, int height, params string[][] layers)
        {
            Image image = Parse(input, width, height);

            for (int i = 0; i < image.Layers.Count; i++)
            {
                var givenLayer = image.Layers[i];
                var expectedLayer = layers[i];

                for (int k = 0; k < givenLayer.Data.Count; k++)
                {
                    Debug.Assert(string.Join("", givenLayer.Data[k]).SequenceEqual(expectedLayer[k]));
                }
            }
        }

        private static Image Parse(string input, int width, int height)
        {
            Image image = new Image();

            int layerLength = width * height;

            for (int i = 0; i < input.Length; i += layerLength)
            {
                Layer layer = new Layer();

                ReadOnlySpan<char> currentLayer = input.AsSpan(i, layerLength);

                for (int k = 0; k < height; k++)
                {
                    ReadOnlySpan<char> span = currentLayer.Slice(k * width, width);
                    List<int> data = new List<int>();
                    for (int l = 0; l < width; l++)
                    {
                        data.Add(int.Parse(span[l].ToString()));
                    }

                    layer.Data.Add(data.ToArray());
                }

                image.Layers.Add(layer);
            }

            return image;
        }

        static string Input = @"222222222222222221222222222022222222222212202120222221222222202222212222200222202021022212222202022022222222222222012222222222222202222022221221222120222222222222222222222222222022222222222222202122222220222222222222112222220222212220022222222212222022222222222222022222222222222212222122221120222222222222222222222221222222222022222222222202212221222220222222202222022222220222202121022222222222022022222222222222022222222222222202222222222020222221222222222222222221222222222122222222222222202122222220222222222222012222222222222022222202202212122022222222222222102222222222222212222022221021222122222222222222222210222222222122222222222212222222222221222222202222022222221222222221122202212202022222222222222222112222222222222202222022222121222102222222222222222201222222222122222222222212222021222222222222222222202222100222222022212222222222222122222222222221002222222222222222222222221222222212222222222222222212222222222122222222222212212020222221222222222222012222011222202111212212202212122122222222222221212222222222222222222122221121222012222222222222222212122222222022222222222212202220222222222222222222202222020222222201212222222212122222222222222221202222222222222222222022221220222100222222222222222222022222222122222222222222222222222221222222222222122222110222222020002212222212122222222222222221122222222222222222222122220121222101222222222222222202122222222022222222222212222021222221222222222222212221122222222021122202202202022022222222222222102222222222222212222122220021222201222222222222222220122222222222222222222202222021222220222222222222222221212222102121012202212222122022222222222221222222222222222222222122221211222110222222222222220221122212222022222222222222212020222221222222202222122220121222112212122212212222222122222222222222202222222222222222222122221020222221222222222222222201122212222222222222222222222122222222222222222222002220222222202210022202212202122222222222222221212222222222222212222122220020222102222222222222220220222212222222222222222212202120222222222222212202202221120222222210022212202212122222222222222222000222222222222222222022220112022121222222222212020201222202220022222222222222212222222222222222202202002222010222212122012212212202122222222222222222121222222222222222222222221211022211222222222222020212222222220222222222222222222120222222222222212212102221002222212002112202212212222122222222222222200222222222222222222012221021222010222222212212121212022222221122222222222212212120222222222222222222122221201222222212212202202222222222222220222222110222222222222222222212221101222021222222222202021212222212222022222222222202212222221220222222222212002222002222102200122212212202122122222222222220112222222222222212222022222010022222222220202212021202122202221222222222222202222022220222222222212212012221222222102002122202222202022122222221222220010222222222222222220202220100222012222222202212120220122222222022222222222212202122222220222222212222122220210222102222212222222202222022222220222220201222222222222212221122220121022122222220212202121212122212222222222221222202212122222220222222202222212222101222202100222222202212222222222222222222221222222222222222221012222200022001222220212222022202022222222222222220222222212020222220222222202212012220101222002021202202202202022022222220222222201222222222222222222022222121222221222221212202220220022202220222222222222212202121221221220202202202022222011222002000112222202212022022222221222220221222202222222202220122222000022020222222212212021221222222220222222220222202212120220221221202202202022220201122202220012212212212122222222220222220011222202222222202221112220111122000222222222222121210022202222222222221222222222122222222221212202212212221021122102200012202222202222222222222222222021222202222222212221202220202122200222021222222021212022222221022220221222222202122222220220222202202222220012222102202002222222212222122222222222220121222212222221022221202220022022120222122212202120211022222222122221222222212202020222222221212202212022222222022102202112222212202222122022201222220021222212222221202220102220202222121222120222212222221022212221122222221222222222021222221220222222212102222002122002111202202212222222122022210222220012222222222220222222002222111122000222220212212220220022202220022222220222202222122220221221202220202202221212122012222222212212212222222222222122221220222212222221002221002220212222110222121212202222200022202222122220221222212212021220222220202220212002222122022202110022202212202122022122221222221011222202222222122221002222121022102222120222212020201222212220022222220222202212021222222222202222222002221210222112211122202222212222022122201022222020222212222222202222002221112122012222222212202220221022202222222221221222222222220222220222220210222112222000222022112112222212212222022222220122220022222212222220202220122222221222002222222222212011222222222222122222221222202212021222222222201221202122220121122012011022222202222122222022220122201020222212222222102221122201222022110222020222222001212122202221222222222222202222020221221221200202202222221211022002010012222222212122222222220022210112222222222221022222012221200222001222222222202212201022212221122220220222212222022220221222202200222112222021122212211012222202212122122022200022220202222112222220222222122202011222002222122222212100211222202220122221221222222222021222221220202211222102222212022102200012212222212022022222221122202111222222222220002222012221022122021222021202222022221222222222022222220222222212020220220222210210222212222120022212020012212212222122222222202022222200222012222221112221222200221022111222220212212121200022222221122220222222222212120221222222200202222012222101122222010002202202202202222222222222222122222002222222002221122210011022112222222222212022212222222220222222221222212202022220221221201200202112221102122212102002212212202122222122222122010112222002221220022221212220002122210222222210212000211222202222022220221222212202122221222220212201212202222022122002101122222202222012122222221222111112222122220220022221112210221222111222222211212100212122222222222220221222222202220222221221222210222122221221222122212222202212202022222122222022021001222122221221202222202220111122120222022201212221200022212221022222222222012212021222221222222200212000221120022112121222212202222022022022202222222211222102221222202220202221222222122222222212212121212022222222222221220222112202221222222221201222222121221202122202221212212212212112022022200022220020222122222222212221012212001222101222121222222211210022202222022220220222212222222222220222220220212202222201222112211012012222222212222022220222201101222002221221122221212222020222220222120212012222200122202220222220220222212202222222220221221222202201221100022202221102022202222202022122201022000101222222220222122220012221222222111222221220022122221022202222222221222222002212020221220221200210202022220210022112212012102202212022122222222122011020222002221222002220112222022222020221220200012111201122212222122222220222222222022202221221221220212212220001222222202212022202212102022122202222220022222112221220222212212220120022020220220202112100211022212220222220222222112212220210220222202211222220222200222112120202222202222002222222211022101122222222221222022212102200012222210222021222012022200122202222022221221222122210222210220222212200212101221110022202022202102212202112222122200022001112222212222222111212102212210222202221020201202021211222222222022220222222122212222110222221220222202200220011222212110112222202222112122022212122221200222102220220120210112222121022200221021201022010200022222221122222222222002210221202222222222222212210021220122222002102112202222112022122222122202121222102221222100210122210101022111221121212222210212222202221022220220222102221222121222222212200222122221121222212000122222222222112222122221122010102222212222222200201222210221222110222120221212121202122202222122221221222122221020102220221200200212011020211222102101102222222222212022022222122101110222102220222000220112222001222012220120222112111120022212220122221222222122200220121221221200200212111122011122012012222212202202202222222211022122001222202220221221202012201021222102221021221222010220022212221222222220222002221121211222220202221212110022111222122000122202222222212122122211122220002222122220222002220022201121222221222121222222100212022202220022220221222002220020221221221202211222021122221222102000112122202202211222022201022212220222002222222211201202200110222220222021220002111111122202222222222221222102210021201220221211200202012020102222122021122022222212101122222220022101200222102221220202222222200112122002222120220112221011122202220022221221222122220022222221222221212222001120222022102002122222202222122222222202202201201222202222222111222212202022022110221220201112011121122222220122220221222112201120210220210201202222110020111222212100222122202022020022122201022202111222202222222111201112201012122222222222201222112112222202220022221221222222222022022220211222222222212122102222002021222222202022211222222202002212111222022222222011200222200220222000220022201102022001122202220222221220222202202122200222200211220202110121012122102121002202212112100222222220102022102222122222222001221002211002022020220121210212122102122122221022221221222222220222000220220201212212101220120222002202002112212012220222022211202110111222212221221111211202200121022210221022222002221222022122220022220222222222201021110221212200201222100020222022002002212202212212010222022211002200011222002222221022220002212220022200221022200022121022222222221212222220222022221022002222212221222202020222201222021022022222202002112022022220112121020222102221222210201122222000222211220121201002100002222212222102220220222112222022221221220220211222101121120122021210212122202002001222222211012220210222202222220201221022220201222202222221202122000111022002222102222220222002220222210220211211222222221222101222112202022222202022110020122212212001200222102221220101210112201010222001222122211202212112122222222122222222222012210221000221012220221222222021012022221111022102202022112211022212212021120222212221220002200112202001222001221021210222200221222022022222222220222202201122011222220211202222122021001022202002222222212222001102112210102120002222102222221102210112210201122102220220220222021112222002210212221222222002222121111222001211211212102121021122202110122022212002120101102222212211022222102220221000200212201220122021220020222212102210222122200102220221222122212021211202020212202222120121111022120021202202202022100220002211202111020222202221221021211002211222022101220221222122101112222122011212221222222122222022221221101201201202111122111222000002212222212222222122222202202111120222122220222201221112202001122110220020212102121201022102020202220221222122220221212200200222212212010022220122200210012112202102110200012221212211001222212221220222222222001000222110221222210002221201022102222022220221222102221121101202202201210202002020122022022112012010202012121001212212212102102222222222021211210222121211222212221020220122011120222112112012222221222022221022102220200212200202002222010122212122102021222022111112112201022010120222012221022211201022020202222020220221202022102022222002012112222222222022212222000212002212221212021221210222021202222201202112110210112202122110220222022221120011201002221220022021220121222222022022022022012122222222222012202121202211221221201202221222000122001201012210202222022111102222212102111222022222120120222212000110222200221221211122210101222112202012220221222012201020220021122221220212002221112212201221012120212222210002222211212201110222002222120011201102102022022211222222221202011200222202211212221220222022210122212011200212210212020122210012101202102220222122111212112211002200022222122222221001211112202222021011220020222122210021022012122222222221222122211220001200011202202222221020020202201200002100222011000200022202012022220202222220220020221212001220222211221222220222111010122002002112220220222222220022011111012222210222220122021112121120222021222121110122202201022111010212222221221201202022102122121120222121200102021202022212011222220220222122212020102121201202202222122221111002212102022010202200201000222200022122210202102221220002221022201112120202222020211112102202222222111022220222222212212020211022221222220222100020211212100102122012212220201021022210202100102202002221022012211122021022020120221221201102222201022122120212220221222112202220210100010220201212220120200222121201002101222012200001212221122201221212122221121100212202100222222200220121201122122021222212212022222222222222220120002100210210212202212211000212112100102122202200002102012211002220100212022220221022212022011211120201222122221002222000222002201212222220222220210022201020010221222222221120121112100022212110202022200121122210012211022222022222122202211202022201122121221021221222120001022222101002220221202112221122000100021222212222101210010102202010102221212210001010122210222012221222222221021210101022120200020220222020201222001000022022010102222222201012212222200222212212210222010200002212021002112120222102200121122222122101102222222220220102000012221121020211220122220202200021222222112222221220212210202022200022110222220202221000220022111112222021202020120110122210212112110212122222222201222212000101021110222220201012221100122012221022220220221102202021111211120220211212011012002022110110222122122102121110012201202010000212002220220212112202022011221112222221221222222010122122200002221220201102200221200210001220210202010000002122200100212101102201222010102222122222110202002220221002000002212200121010220020201122201212022112222002120221201201222020010001210211222202021210012202020001012000122112000121002212002122212202112222122110021202200111120222222022220212121100022012000002022220201020212121221011210222221202001022122012220221002202012120001212122221002022100202112222222101222022110211220000221220200102022102122022112222222222011100222221210220020222202212220020212222220211012112112202201102112212202211012202022222220221212202120111220111220020202000012100122012120012221222100112200122100011001211200202222012020202101121212112022220002110212211022210000212212221021211212112022120120101220220221022221110022122211202121220101112210122101221100000212202221202102002001111022212222000020020222221122220012212102222122100000202210110021211220220211112020012022022202212220220110111221022200011221212200202110112222202212112202002202010011120002201112110220202012222122110021102022211221221220020211002101110222112111102220222212210200221110101002022210212100011002002201001102200102211220000112200022201100222020221122212000212210110220011221022222201210121022022021202020122022110222221202101210200210202112022120021210202202010222000001122012211112201002202112221220101001022010012120002220221212202000010222022202122121020211111222020100202100101221222021101011110122222122211012220221020022212022102212202111221222110222112102122121021012102112102102221101010211220101200201111022202221202221212110101110212020010211211200122020102112020200102201121010010200100202112211201010000211012";
    }

    public class Layer
    {
        public List<int[]> Data { get; } = new List<int[]>();
    }

    public class Image
    {
        public List<Layer> Layers { get; } = new List<Layer>();
    }
}
